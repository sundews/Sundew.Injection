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
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Extensions;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

internal static class FactoryResolvedGraphProvider
{
    private const string RootNodeName = "root";

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
                ImmutableArray.Create(compilationData.LifecycleHandlerBinding),
                new KnownEnumerableTypes(compilationData.IEnumerableOfTType, compilationData.IReadOnlyListOfTType));
            var scopeResolverBuilder = new ScopeResolverBuilder(bindingResolver, injectionDefinition.RequiredParameterScopes, injectionDefinition.FactoryCreationDefinitions);
            foreach (var factoryCreationDefinition in injectionDefinition.FactoryCreationDefinitions)
            {
                var useTargetTypeNameForCreateMethod =
                    factoryCreationDefinition.FactoryMethodRegistrations.Count > 1;
                var factoryConstructorParameters = ImmutableList.CreateBuilder<FactoryConstructorParameter>();
                var needsLifecycleHandling = false;
                bindingResolver.RegisterThisFactory(factoryCreationDefinition.FactoryType, factoryCreationDefinition.FactoryInterfaceType);
                var factoryMethodRegistrationsResult = factoryCreationDefinition.FactoryMethodRegistrations.AllOrFailed(x =>
                {
                    var createBindingResult = bindingResolver.CreateBindingRoot(x, useTargetTypeNameForCreateMethod)
                        .WithError(bindingError => ImmutableList.Create<InjectionStageError>(
                            new InjectionStageError.ResolveTypeError(bindingError, RootNodeName)));
                    if (createBindingResult.TryGetError(out var bindingErrors))
                    {
                        return Item.Fail(bindingErrors).Omits<FactoryMethodData>();
                    }

                    var rootBinding = createBindingResult.Value.Binding;
                    var scopeResolverResult = scopeResolverBuilder.Build(rootBinding);
                    if (scopeResolverResult.TryGetError(out var scopeErrors))
                    {
                        return Item.Fail(scopeErrors.Select(x =>
                        {
                            return x switch
                            {
                                ResolvedBindingError.Error error => new InjectionStageError.ResolveTypeError(error.BindingError, RootNodeName),
                                ResolvedBindingError.ParameterError parameterError => InjectionStageError._ResolveParameterError(parameterError.Type, parameterError.ParameterName, parameterError.ParameterSources),
                            };
                        }).ToImmutableList());
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
                        (createBindingResult.Value.ReturnType, x.Return.TypeMetadata),
                        (rootBinding.TargetType, x.Target.TypeMetadata),
                        injectionTreeResult.Value.Root,
                        injectionTreeResult.Value.RootNeedsLifecycleHandling));
                });

                if (factoryMethodRegistrationsResult.TryGet(out var all, out var failed))
                {
                    var (factoryType, factoryInterfaceType) = bindingResolver.CreateFactoryBinding(factoryCreationDefinition, factoryConstructorParameters, needsLifecycleHandling);

                    var lifecycleInjectionNodeResult = TryCreateLifecycleInjectionNode(
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
                        factoryCreationDefinition.Accessibility,
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
        bool needsLifecycleHandling,
        CompilationData compilationData,
        ScopeResolverBuilder scopeResolverBuilder,
        BindingResolver bindingResolver,
        RequiredParametersInjectionResolver requiredParametersInjectionResolver,
        CancellationToken cancellationToken)
    {
        if (needsLifecycleHandling)
        {
            var rootBinding = compilationData.LifecycleHandlerBinding;
            var scopeResolverResult = scopeResolverBuilder.Build(rootBinding);
            if (scopeResolverResult.TryGetError(out var bindingErrors))
            {
                return R.Error(bindingErrors.Select(x =>
                {
                    return x switch
                    {
                        ResolvedBindingError.Error error => new InjectionStageError.ResolveTypeError(error.BindingError, RootNodeName),
                        ResolvedBindingError.ParameterError parameterError => InjectionStageError._ResolveParameterError(parameterError.Type, parameterError.ParameterName, parameterError.ParameterSources),
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
        return injectionStageErrors switch
        {
            InjectionStageError.UnsupportedInstanceMethod unsupportedInstanceMethod => Diagnostic.Create(Diagnostics.UnsupportedInstanceMethodError, Location.None, unsupportedInstanceMethod.Type, unsupportedInstanceMethod.Type, unsupportedInstanceMethod.DependeeNodeName),
            InjectionStageError.ResolveParameterError resolveRequiredParameterError => Diagnostic.Create(Diagnostics.ResolveRequiredParameterError, Location.None, resolveRequiredParameterError.Type, resolveRequiredParameterError.DependeeNodeName),
            InjectionStageError.ScopeError scopeError => Diagnostic.Create(Diagnostics.ScopeError, Location.None, scopeError.DefiniteType.FullName, scopeError.Scope, scopeError.DependeeNodeName, scopeError.DependeeScope),
            InjectionStageError.ResolveTypeError resolveTypeError => Diagnostic.Create(Diagnostics.ResolveTypeError, Location.None, resolveTypeError.BindingError, resolveTypeError.DependeeNodeName),
        };
    }
}