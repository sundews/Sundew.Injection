// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryDataProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage;

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Extensions;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal static class FactoryDataProvider
{
    public static IncrementalValuesProvider<R<FactoryData, ValueList<Diagnostic>>> SetupFactoryDataStage(this IncrementalValuesProvider<(InjectionDefinition InjectionDefinition, CompilationData CompilationData, ImmutableArray<SyntaxNode> AccessibleConstructors)> tupleForCodeGenerationProvider)
    {
        return tupleForCodeGenerationProvider.SelectMany((valueProvider, cancellationToken) =>
        {
            var factoryDefinitionResults = ImmutableArray.CreateBuilder<R<FactoryData, ValueList<Diagnostic>>>();
            try
            {
                var injectionDefinition = valueProvider.InjectionDefinition;
                var compilationData = valueProvider.CompilationData;
                var requiredParametersInjectionResolver = new RequiredParametersInjectionResolver(
                    injectionDefinition.RequiredParameterInjection, injectionDefinition.RequiredParameters);
                var bindingResolver = new BindingResolver(injectionDefinition.BindingRegistrations, injectionDefinition.GenericBindingRegistrations);
                var scopeResolverBuilder = new ScopeResolverBuilder(bindingResolver);
                foreach (var factoryCreationDefinition in injectionDefinition.FactoryDefinitions)
                {
                    var useTargetTypeNameForCreateMethod =
                        factoryCreationDefinition.FactoryMethodRegistrations.Count > 1;
                    var factoryConstructorParameters = ImmutableList.CreateBuilder<FactoryConstructorParameterInjectionNode>();
                    var implementDisposable = false;
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
                        var injectionTreeBuilder = new InjectionTreeBuilder(bindingResolver, requiredParametersInjectionResolver, scopeResolver, compilationData);
                        var injectionTreeResult = injectionTreeBuilder.Build(rootBinding, cancellationToken);
                        if (!injectionTreeResult.IsSuccess)
                        {
                            return Item.Fail<FactoryMethodData, ImmutableList<InjectionStageError>>(injectionTreeResult.Error);
                        }

                        BooleanHelper.SetIfTrue(ref implementDisposable, injectionTreeResult.Value.ImplementDisposable);
                        factoryConstructorParameters.AddRange(injectionTreeResult.Value.FactoryConstructorParameters);
                        return Item.Pass(new FactoryMethodData(
                            rootBinding.Method.Name,
                            (bindingReturnType, x.Return.TypeMetadata),
                            (rootBinding.TargetType, x.Target.TypeMetadata),
                            injectionTreeResult.Value.Root));
                    });

                    if (factoryMethodInfoResult.TryGet(out var all, out var failed))
                    {
                        var (factoryType, factoryInterfaceType) = bindingResolver.CreateFactoryBinding(factoryCreationDefinition, all.First(), factoryConstructorParameters, implementDisposable, compilationData.AssemblyName);

                        var factoryDefinition = new FactoryData(
                            factoryType,
                            factoryInterfaceType,
                            factoryCreationDefinition.GenerateInterface,
                            factoryCreationDefinition.Accessibility,
                            all.Items.ToImmutableArray());

                        factoryDefinitionResults.Add(R.Success(factoryDefinition));
                    }
                    else
                    {
                        factoryDefinitionResults.Add(R.Error(failed.GetErrors().SelectMany(x => x).Select(GetDiagnostic).ToImmutableArray().ToValueList()));
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
                factoryDefinitionResults.Add(R.Error(ImmutableArray.Create(Diagnostic.Create(Diagnostics.UnknownError, default, e.ToString())).ToValueList()));
                return factoryDefinitionResults.ToImmutable();
            }
        });
    }

    private static Diagnostic GetDiagnostic(InjectionStageError injectionStageErrors)
    {
        return injectionStageErrors switch
        {
            InjectionStageError.ResolveParameterError resolveRequiredParameterError => Diagnostic.Create(Diagnostics.ResolveRequiredParameterError, Location.None, resolveRequiredParameterError.BindingError, resolveRequiredParameterError.ParentNode),
            InjectionStageError.ScopeError scopeError => Diagnostic.Create(Diagnostics.ScopeError, Location.None, scopeError.DefiniteType.FullName, scopeError.Scope, scopeError.ParentNode, scopeError.ParentScope),
            InjectionStageError.ResolveTypeError resolveTypeError => Diagnostic.Create(Diagnostics.ResolveTypeError, Location.None, resolveTypeError.BindingError, resolveTypeError.ParentNode),
        };
    }
}