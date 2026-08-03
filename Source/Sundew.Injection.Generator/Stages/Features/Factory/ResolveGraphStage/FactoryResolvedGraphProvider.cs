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
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using CreateGenericMethodError = Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers.CreateGenericMethodError;
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
            var bindingResolver = new BindingResolver(
                new BindingRegistrationResolver(injectionDefinition.FallbackBindingRegistrations, injectionDefinition.BindingRegistrations),
                injectionDefinition.GenericBindingRegistrations,
                ImmutableArray.Create(compilationData.ProvidedSundewInjectionCompilationData.LifecycleHandlerBinding),
                new KnownEnumerableTypes(compilationData.IEnumerableOfTType, compilationData.IReadOnlyListOfTType));
            foreach (var factoryImplementationDefinition in injectionDefinition.FactoryImplementationDefinitions)
            {
                var scopeResolverBuilder = new ScopeResolverBuilder(bindingResolver, factoryImplementationDefinition.ParameterSources, injectionDefinition.FactoryImplementationDefinitions);
                var factoryConstructorParametersInjectionResolver = new ParametersInjectionResolver(factoryImplementationDefinition.ParameterSources);

                var lifecycle = Lifecycle.None;
                bindingResolver.RegisterThisFactory(factoryImplementationDefinition.FactoryType, factoryImplementationDefinition.FactoryInterfaceType);
                var factoryMethodRegistrationsResult = factoryImplementationDefinition.FactoryMethodRegistrations.AllOrFailed(factoryMethodRegistrationPair =>
                {
                    var factoryMethodRegistrationPairsResult = factoryMethodRegistrationPair.Value.AllOrFailed(factoryMethodRegistration =>
                    {
                        var factoryMethodParametersInjectionResolver = new ParametersInjectionResolver(factoryMethodRegistration.ParameterSources, factoryConstructorParametersInjectionResolver);
                        var bindingRoot = bindingResolver.CreateBindingRoot(factoryMethodRegistration);
                        var rootBinding = bindingRoot.Binding;
                        var scopeResolverResult = scopeResolverBuilder.Build(factoryImplementationDefinition.FactoryType, rootBinding, factoryMethodParametersInjectionResolver);
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
                            }).ToImmutableList()).Omits<ResolvedRootFactoryMethod>();
                        }

                        var injectionTreeBuilder = new InjectionTreeBuilder(bindingResolver, factoryMethodParametersInjectionResolver, scopeResolverResult.Value);
                        var injectionTreeResult = injectionTreeBuilder.Build(rootBinding, bindingRoot.ReturnType, cancellationToken);
                        if (injectionTreeResult.TryGetError(out var injectionErrors))
                        {
                            return Item.Fail(injectionErrors);
                        }

                        lifecycle |= injectionTreeResult.Value.Lifecycle;

                        // BooleanHelper.SetIfTrue(ref needsLifecycleHandling, injectionTreeResult.Value.NeedsLifecycleHandling);
                        return Item.Pass(new ResolvedRootFactoryMethod(
                            factoryMethodRegistration.FactoryMethodTarget.IsPartialDefinition,
                            factoryMethodRegistration.FactoryMethodTarget.IsProperty,
                            factoryMethodRegistration.FactoryMethodTarget.Method.Name,
                            factoryMethodRegistration.Return with { Type = bindingRoot.ReturnType },
                            factoryMethodRegistration.Target with { Type = rootBinding.TargetType },
                            injectionTreeResult.Value.Root,
                            factoryMethodRegistration.FactoryMethodTarget.Method.Parameters,
                            injectionTreeResult.Value.RootLifecycle));
                    });

                    if (factoryMethodRegistrationPairsResult.TryGetError(out var failed, out var resolvedRootFactoryMethods))
                    {
                        return Item.Fail(failed.GetErrors()).Omits<(NamedType ContainingType, ValueArray<ResolvedRootFactoryMethod>)>();
                    }

                    return Item.Pass((factoryMethodRegistrationPair.Key, resolvedRootFactoryMethods.ToValueArray()));
                });

                if (factoryMethodRegistrationsResult.TryGet(out var all, out var failed2))
                {
                    var (factoryType, factoryInterfaceType) = bindingResolver.CreateFactoryBinding(factoryImplementationDefinition, factoryImplementationDefinition.DeclaredConstructor.Parameters, lifecycle);

                    var lifecycleInjectionNodeResult = TryCreateLifecycleInjectionNode(
                        factoryImplementationDefinition.FactoryType,
                        lifecycle,
                        compilationData,
                        scopeResolverBuilder,
                        bindingResolver,
                        factoryConstructorParametersInjectionResolver,
                        cancellationToken);
                    if (lifecycleInjectionNodeResult.TryGetError(out var diagnostics, out var lifecycleInjectionNode))
                    {
                        factoryDefinitionResults.Add(R.Error(new Diagnostics(diagnostics)));
                        break;
                    }

                    var factoryDefinition = new FactoryResolvedGraph(
                        factoryType,
                        factoryInterfaceType,
                        factoryImplementationDefinition.DeclaredConstructor,
                        factoryImplementationDefinition.Accessibility,
                        lifecycle,
                        lifecycleInjectionNode,
                        scopeResolverBuilder.Build(),
                        all.Items.ToImmutableDictionary(x => x.ContainingType, x => x.Item2));

                    factoryDefinitionResults.Add(R.Success(factoryDefinition));
                }
                else
                {
                    factoryDefinitionResults.Add(R.Error(
                        new Diagnostics(
                            failed2.GetErrors()
                                .SelectMany(x => x.SelectMany(x => x))
                                .Select(GetDiagnostic).ToImmutableArray())));
                }

                /*

                var factoryMethodRegistration = factoryMethodRegistrationPair.Value;
                var factoryMethodParametersInjectionResolver = new ParametersInjectionResolver(factoryMethodRegistration.ParameterSources, factoryConstructorParametersInjectionResolver);
                var bindingRoot = bindingResolver.CreateBindingRoot(factoryMethodRegistration);
                var rootBinding = bindingRoot.Binding;
                var scopeResolverResult = scopeResolverBuilder.Build(factoryImplementationDefinition.FactoryType, rootBinding, factoryMethodParametersInjectionResolver);
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
                    }).ToImmutableList()).Omits<(NamedType, ResolvedRootFactoryMethod)>();
                }

                var injectionTreeBuilder = new InjectionTreeBuilder(bindingResolver, factoryMethodParametersInjectionResolver, scopeResolverResult.Value);
                var injectionTreeResult = injectionTreeBuilder.Build(rootBinding, bindingRoot.ReturnType, cancellationToken);
                if (injectionTreeResult.TryGetError(out var injectionErrors))
                {
                    return Item.Fail(injectionErrors);
                }

                BooleanHelper.SetIfTrue(ref needsLifecycleHandling, injectionTreeResult.Value.NeedsLifecycleHandling);
                return Item.Pass((factoryMethodRegistrationPair.Key, new ResolvedRootFactoryMethod(
                    factoryMethodRegistration.FactoryMethodTarget.IsPartialDefinition,
                    factoryMethodRegistration.FactoryMethodTarget.IsProperty,
                    factoryMethodRegistration.FactoryMethodTarget.Method.Name,
                    factoryMethodRegistration.Return with { Type = bindingRoot.ReturnType },
                    factoryMethodRegistration.Target with { Type = rootBinding.TargetType },
                    injectionTreeResult.Value.Root,
                    factoryMethodRegistration.FactoryMethodTarget.Method.Parameters,
                    injectionTreeResult.Value.RootNeedsLifecycleHandling)));
            });

            if (factoryMethodRegistrationsResult.TryGet(out var all, out var failed))
            {
                var (factoryType, factoryInterfaceType) = bindingResolver.CreateFactoryBinding(factoryImplementationDefinition, factoryImplementationDefinition.Parameters, needsLifecycleHandling);

                var lifecycleInjectionNodeResult = TryCreateLifecycleInjectionNode(
                    factoryImplementationDefinition.FactoryType,
                    needsLifecycleHandling,
                    compilationData,
                    scopeResolverBuilder,
                    bindingResolver,
                    factoryConstructorParametersInjectionResolver,
                    cancellationToken);
                if (lifecycleInjectionNodeResult.TryGetError(out var diagnostics, out var lifecycleInjectionNode))
                {
                    factoryDefinitionResults.Add(R.Error(new Diagnostics(diagnostics)));
                    break;
                }

                var factoryDefinition = new FactoryResolvedGraph(
                    factoryType,
                    factoryInterfaceType,
                    factoryImplementationDefinition.Parameters,
                    factoryImplementationDefinition.HasConstructorMethod,
                    factoryImplementationDefinition.Accessibility,
                    needsLifecycleHandling,
                    lifecycleInjectionNode,
                    all.Items.ToImmutableDictionary(x => x.Item1, x => x.Item2));

                factoryDefinitionResults.Add(R.Success(factoryDefinition));
            }
            else
            {
                factoryDefinitionResults.Add(R.Error(
                    new Diagnostics(
                        failed.GetErrors()
                            .SelectMany(x => x)
                            .Select(GetDiagnostic).ToImmutableArray())));
            }*/
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
        Lifecycle lifecycle,
        CompilationData compilationData,
        ScopeResolverBuilder scopeResolverBuilder,
        BindingResolver bindingResolver,
        ParametersInjectionResolver parametersInjectionResolver,
        CancellationToken cancellationToken)
    {
        if (lifecycle != Lifecycle.None)
        {
            var rootBinding = compilationData.ProvidedSundewInjectionCompilationData.LifecycleHandlerBinding;
            var scopeResolverResult = scopeResolverBuilder.Build(factoryType, rootBinding, parametersInjectionResolver);
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

            var injectionTreeBuilder = new InjectionTreeBuilder(bindingResolver, parametersInjectionResolver, scopeResolverResult.Value);
            var injectionTreeResult = injectionTreeBuilder.Build(rootBinding, rootBinding.ReferencedType, cancellationToken);
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
        const string separator = ", ";
        return injectionStageErrors switch
        {
            InjectionStageError.UnsupportedInstanceMethodError unsupportedInstanceMethod => Diagnostic.Create(
                Diagnostics.UnsupportedInstanceMethodError,
                Location.None,
                unsupportedInstanceMethod.Type.Name,
                unsupportedInstanceMethod.Type.Name,
                unsupportedInstanceMethod.DependantNodeName),
            InjectionStageError.ResolveParameterError resolveRequiredParameterError => Diagnostic.Create(
                Diagnostics.ResolveRequiredParameterError,
                Location.None,
                resolveRequiredParameterError.Type.Name,
                resolveRequiredParameterError.DependantNodeName,
                string.Join(separator + Environment.NewLine, resolveRequiredParameterError.ParameterSources)),
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
            InjectionStageError.ReferencedTypeMismatchError referencedTypeMismatchError => Diagnostic.Create(Diagnostics.ReferencedTypeMismatchError, Location.None, referencedTypeMismatchError.ActualParameterType.Name, referencedTypeMismatchError.ReferencedType.Name, referencedTypeMismatchError.DependantNodeName),
        };
    }
}