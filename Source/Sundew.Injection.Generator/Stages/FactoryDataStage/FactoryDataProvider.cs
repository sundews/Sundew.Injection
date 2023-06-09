﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryDataProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Primitives.Computation;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Extensions;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

internal static class FactoryDataProvider
{
    public static IncrementalValuesProvider<R<FactoryData, ValueList<Diagnostic>>> SetupFactoryDataStage(this IncrementalValuesProvider<(InjectionDefinition InjectionDefinition, CompilationData CompilationData, ImmutableArray<SyntaxNode> AccessibleConstructors)> tupleForCodeGenerationProvider)
    {
        return tupleForCodeGenerationProvider.SelectMany((x, cancellationToken) => GetFactoryData(x.InjectionDefinition, x.CompilationData, cancellationToken));
    }

    internal static ImmutableArray<R<FactoryData, ValueList<Diagnostic>>> GetFactoryData(
        InjectionDefinition injectionDefinition, CompilationData compilationData, CancellationToken cancellationToken)
    {
        var factoryDefinitionResults = ImmutableArray.CreateBuilder<R<FactoryData, ValueList<Diagnostic>>>();
        try
        {
            var requiredParametersInjectionResolver = new RequiredParametersInjectionResolver(
                injectionDefinition.RequiredParameterInjection, injectionDefinition.RequiredParameters);
            var bindingResolver = new BindingResolver(injectionDefinition.BindingRegistrations, injectionDefinition.GenericBindingRegistrations);
            var scopeResolverBuilder = new ScopeResolverBuilder(bindingResolver);
            foreach (var factoryCreationDefinition in injectionDefinition.FactoryDefinitions)
            {
                var useTargetTypeNameForCreateMethod =
                    factoryCreationDefinition.FactoryMethodRegistrations.Count > 1;
                var factoryConstructorParameters = ImmutableList.CreateBuilder<FactoryConstructorParameterInjectionNode>();
                var needsLifecycleHandling = false;
                var factoryMethodInfoResult = factoryCreationDefinition.FactoryMethodRegistrations.AllOrFailed(x =>
                {
                    var createBindingResult = bindingResolver.CreateBindingRoot(x, useTargetTypeNameForCreateMethod);
                    if (!createBindingResult.IsSuccess)
                    {
                        return Item.Fail<FactoryMethodData, ImmutableList<InjectionStageError>>(
                            ImmutableList.Create<InjectionStageError>(
                                new InjectionStageError.ResolveTypeError(createBindingResult.Error, "root")));
                    }

                    var rootBinding = createBindingResult.Value.Binding;
                    var bindingReturnType = createBindingResult.Value.ReturnType;
                    var scopeResolver = scopeResolverBuilder.Build(rootBinding);
                    var injectionTreeBuilder = new InjectionTreeBuilder(bindingResolver, requiredParametersInjectionResolver, scopeResolver);
                    var injectionTreeResult = injectionTreeBuilder.Build(rootBinding, cancellationToken);
                    if (!injectionTreeResult.IsSuccess)
                    {
                        return Item.Fail<FactoryMethodData, ImmutableList<InjectionStageError>>(injectionTreeResult.Error);
                    }

                    BooleanHelper.SetIfTrue(ref needsLifecycleHandling, injectionTreeResult.Value.NeedsLifecycleHandling);
                    factoryConstructorParameters.AddRange(injectionTreeResult.Value.FactoryConstructorParameters);
                    return Item.Pass(new FactoryMethodData(
                        rootBinding.Method.Name,
                        (bindingReturnType, x.Return.TypeMetadata),
                        (rootBinding.TargetType, x.Target.TypeMetadata),
                        injectionTreeResult.Value.Root));
                });

                switch (factoryMethodInfoResult)
                {
                    case All<FactoryMethodRegistration, FactoryMethodData, ImmutableList<InjectionStageError>> all:

                        var (factoryType, factoryInterfaceType) = bindingResolver.CreateFactoryBinding(factoryCreationDefinition, all.First(), factoryConstructorParameters, needsLifecycleHandling, compilationData.AssemblyName);

                        var factoryDefinition = new FactoryData(
                            factoryType,
                            factoryInterfaceType,
                            factoryCreationDefinition.GenerateInterface,
                            factoryCreationDefinition.Accessibility,
                            needsLifecycleHandling,
                            all.Items.ToImmutableArray());

                        factoryDefinitionResults.Add(R.Success(factoryDefinition));
                        break;
                    case Failed<FactoryMethodRegistration, FactoryMethodData, ImmutableList<InjectionStageError>>
                        failedItems:
                        factoryDefinitionResults.Add(R.Error(failedItems.GetErrors().SelectMany(x => x)
                            .Select(GetDiagnostic).ToImmutableArray().ToValueList()));
                        break;
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
            factoryDefinitionResults.Add(R.Error(ImmutableArray
                .Create(Diagnostic.Create(Diagnostics.UnknownError, default, e.ToString())).ToValueList()));
            return factoryDefinitionResults.ToImmutable();
        }
    }

    private static Diagnostic GetDiagnostic(InjectionStageError injectionStageErrors)
    {
        return injectionStageErrors switch
        {
            InjectionStageError.ResolveParameterError resolveRequiredParameterError => Diagnostic.Create(Diagnostics.ResolveRequiredParameterError, Location.None, resolveRequiredParameterError.Type, resolveRequiredParameterError.ParentNode),
            InjectionStageError.ScopeError scopeError => Diagnostic.Create(Diagnostics.ScopeError, Location.None, scopeError.DefiniteType.FullName, scopeError.Scope, scopeError.ParentNode, scopeError.ParentScope),
            InjectionStageError.ResolveTypeError resolveTypeError => Diagnostic.Create(Diagnostics.ResolveTypeError, Location.None, resolveTypeError.BindingError, resolveTypeError.ParentNode),
        };
    }
}