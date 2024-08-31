// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryResolvedGraphProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Linq;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Extensions;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

internal static class FactoryResolvedGraphProvider
{
    public static IncrementalValuesProvider<R<FactoryResolvedGraph, Diagnostics>> SetupResolveFactoryGraphStage(
        this IncrementalValuesProvider<(InjectionDefinition InjectionDefinition, CompilationData CompilationData, ImmutableArray<SyntaxNode> AccessibleConstructors)> tupleForCodeGenerationProvider)
    {
        return tupleForCodeGenerationProvider.SelectMany((x, cancellationToken) => GetResolvedFactoryGraph(x.InjectionDefinition, x.CompilationData, cancellationToken));
    }

    internal static ImmutableArray<R<FactoryResolvedGraph, Diagnostics>> GetResolvedFactoryGraph(
        InjectionDefinition injectionDefinition, CompilationData compilationData, CancellationToken cancellationToken)
    {
        var factoryDefinitionResults = ImmutableArray.CreateBuilder<R<FactoryResolvedGraph, Diagnostics>>();
        try
        {
            var requiredParametersInjectionResolver = new RequiredParametersInjectionResolver(
                injectionDefinition.RequiredParameterInjection, injectionDefinition.RequiredParameterSources);
            var bindingResolver = new BindingResolver(
                injectionDefinition.BindingRegistrations,
                injectionDefinition.GenericBindingRegistrations,
                requiredParametersInjectionResolver,
                ImmutableArray.Create(compilationData.ProvidedSundewInjectionCompilationData.LifecycleHandlerBinding),
                new KnownEnumerableTypes(compilationData.IEnumerableOfTType, compilationData.IReadOnlyListOfTType));
            var scopeResolverBuilder = new ScopeResolverBuilder(bindingResolver, injectionDefinition.RequiredParameterScopes, injectionDefinition.FactoryImplementationDefinitions);
            foreach (var factoryImplementationDefinition in injectionDefinition.FactoryImplementationDefinitions)
            {
                var useTargetTypeNameForCreateMethod =
                    factoryImplementationDefinition.FactoryMethodRegistrations.Count > 1;
                var factoryConstructorParameters = ImmutableList.CreateBuilder<FactoryConstructorParameter>();
                var needsLifecycleHandling = false;
                bindingResolver.RegisterThisFactory(factoryImplementationDefinition.FactoryType, factoryImplementationDefinition.FactoryInterfaceType);
                var factoryMethodRegistrationsResult = factoryImplementationDefinition.FactoryMethodRegistrations.AllOrFailed(factoryMethodRegistration =>
                {
                    var bindingRoot = bindingResolver.CreateBindingRoot(factoryMethodRegistration, useTargetTypeNameForCreateMethod);
                    var rootBinding = bindingRoot.Binding;
                    var scopeResolverResult = scopeResolverBuilder.Build(factoryImplementationDefinition.FactoryType, rootBinding);
                    if (scopeResolverResult.TryGetError(out var scopeErrors))
                    {
                        return Item.Fail(scopeErrors.Select(x =>
                        {
                            return x switch
                            {
                                CreateGenericMethodError error => InjectionStageError._CreateGenericMethodError(error.Error, factoryImplementationDefinition.FactoryType.Name),
                                ParameterError parameterError => InjectionStageError._ResolveParameterError(parameterError.Type, parameterError.ParameterName, parameterError.ParameterSources),
                                ScopeError scopeError => InjectionStageError._ScopeError(scopeError.CurrentType, scopeError.CurrentScope, scopeError.Dependant.Type.Name, scopeError.Dependant.Scope.GetType().Name),
                            };
                        }).ToImmutableList()).Omits<FactoryMethodData>();
                    }

                    var injectionTreeBuilder = new InjectionTreeBuilder(bindingResolver, requiredParametersInjectionResolver, scopeResolverResult.Value);
                    var injectionTreeResult = injectionTreeBuilder.Build(rootBinding, cancellationToken);
                    if (injectionTreeResult.TryGetError(out var injectionErrors))
                    {
                        return Item.Fail(injectionErrors);
                    }

                    BooleanHelper.SetIfTrue(ref needsLifecycleHandling, injectionTreeResult.Value.NeedsLifecycleHandling);
                    factoryConstructorParameters.AddRange(injectionTreeResult.Value.FactoryConstructorParameters);
                    return Item.Pass(new FactoryMethodData(
                        rootBinding.Method.Name,
                        factoryMethodRegistration.Return with { Type = bindingRoot.ReturnType },
                        factoryMethodRegistration.Target with { Type = rootBinding.TargetType },
                        injectionTreeResult.Value.Root,
                        injectionTreeResult.Value.RootNeedsLifecycleHandling));
                });

                if (factoryMethodRegistrationsResult.TryGet(out var all, out var failed))
                {
                    var (factoryType, factoryInterfaceType) = bindingResolver.CreateFactoryBinding(factoryImplementationDefinition, factoryConstructorParameters, needsLifecycleHandling);

                    var lifecycleInjectionNodeResult = TryCreateLifecycleInjectionNode(
                        factoryImplementationDefinition.FactoryType,
                        needsLifecycleHandling,
                        compilationData,
                        scopeResolverBuilder,
                        bindingResolver,
                        requiredParametersInjectionResolver,
                        cancellationToken);
                    if (lifecycleInjectionNodeResult.TryGetError(out var diagnostics))
                    {
                        factoryDefinitionResults.Add(R.Error(new Diagnostics(diagnostics)));
                        break;
                    }

                    var factoryDefinition = new FactoryResolvedGraph(
                        factoryType,
                        factoryInterfaceType,
                        factoryImplementationDefinition.Accessibility,
                        needsLifecycleHandling,
                        lifecycleInjectionNodeResult.Value,
                        all.Items.ToImmutableArray());

                    factoryDefinitionResults.Add(R.Success(factoryDefinition));
                }
                else
                {
                    factoryDefinitionResults.Add(R.Error(
                        new Diagnostics(
                            failed.GetErrors()
                                .SelectMany(x => x)
                                .Select(GetDiagnostic).ToImmutableArray())));
                }
            }

            return factoryDefinitionResults.ToImmutable();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            factoryDefinitionResults.Add(R.Error(new Diagnostics(Diagnostic.Create(Diagnostics.UnknownError, default, e.ToString()))));
            return factoryDefinitionResults.ToImmutable();
        }
    }

    private static R<InjectionTree?, ImmutableList<Diagnostic>> TryCreateLifecycleInjectionNode(
        Type factoryType,
        bool needsLifecycleHandling,
        CompilationData compilationData,
        ScopeResolverBuilder scopeResolverBuilder,
        BindingResolver bindingResolver,
        RequiredParametersInjectionResolver requiredParametersInjectionResolver,
        CancellationToken cancellationToken)
    {
        if (needsLifecycleHandling)
        {
            var rootBinding = compilationData.ProvidedSundewInjectionCompilationData.LifecycleHandlerBinding;
            var scopeResolverResult = scopeResolverBuilder.Build(factoryType, rootBinding);
            if (scopeResolverResult.TryGetError(out var bindingErrors))
            {
                return R.Error(bindingErrors.Select(x =>
                {
                    return x switch
                    {
                        CreateGenericMethodError error => InjectionStageError._CreateGenericMethodError(error.Error, factoryType.Name),
                        ParameterError parameterError => InjectionStageError._ResolveParameterError(parameterError.Type, parameterError.ParameterName, parameterError.ParameterSources),
                        ScopeError scopeError => InjectionStageError._ScopeError(scopeError.CurrentType, scopeError.CurrentScope, scopeError.Dependant.Type.Name, scopeError.Dependant.Scope.ToString()),
                    };
                }).Select(GetDiagnostic).ToImmutableList());
            }

            var injectionTreeBuilder = new InjectionTreeBuilder(bindingResolver, requiredParametersInjectionResolver, scopeResolverResult.Value);
            var injectionTreeResult = injectionTreeBuilder.Build(rootBinding, cancellationToken);
            if (injectionTreeResult.TryGetError(out var injectionStageErrors))
            {
                return R.Error(injectionStageErrors.Select(GetDiagnostic).ToImmutableList());
            }

            return R.SuccessOption(injectionTreeResult.Value);
        }

        return R.Success();
    }

    private static Diagnostic GetDiagnostic(InjectionStageError injectionStageErrors)
    {
        const string mappingSign = " => ";
        return injectionStageErrors switch
        {
            InjectionStageError.UnsupportedInstanceMethodError unsupportedInstanceMethod => Diagnostic.Create(
                Diagnostics.UnsupportedInstanceMethodError,
                Location.None,
                unsupportedInstanceMethod.Type,
                unsupportedInstanceMethod.Type,
                unsupportedInstanceMethod.DependantNodeName),
            InjectionStageError.ResolveParameterError resolveRequiredParameterError => Diagnostic.Create(
                Diagnostics.ResolveRequiredParameterError,
                Location.None,
                resolveRequiredParameterError.Type,
                resolveRequiredParameterError.DependantNodeName),
            InjectionStageError.ScopeError scopeError => Diagnostic.Create(
                Diagnostics.ScopeError,
                scopeError.Scope.Location,
                scopeError.Type.Name,
                scopeError.Scope.GetType().Name,
                scopeError.DependantNodeName,
                scopeError.DependantScope),
            InjectionStageError.CreateGenericMethodError createGenericMethodError => Diagnostic.Create(
                Diagnostics.CreateGenericMethodError,
                Location.None,
                createGenericMethodError.Error.ContainingType.Name,
                createGenericMethodError.Error.Name,
                createGenericMethodError.Error.FailedParameters.JoinToString((builder, tuple) => builder.Append(tuple.TargetParameter.Name).Append(mappingSign).Append(tuple.UnresolvedSymbol.Name), ','),
                createGenericMethodError.DependantNodeName),
        };
    }
}