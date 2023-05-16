// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionTreeBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Primitives.Computation;
using Sundew.DiscriminatedUnions;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Extensions;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers;
using Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using Scope = Sundew.Injection.Generator.TypeSystem.Scope;

internal sealed class InjectionTreeBuilder
{
    private const string Root = "<root>";
    private readonly BindingResolver bindingResolver;
    private readonly RequiredParametersInjectionResolver requiredParametersInjectionResolver;
    private readonly ScopeResolver scopeResolver;
    private readonly CompilationData compilationData;

    public InjectionTreeBuilder(BindingResolver bindingResolver, RequiredParametersInjectionResolver requiredParametersInjectionResolver, ScopeResolver scopeResolver, CompilationData compilationData)
    {
        this.bindingResolver = bindingResolver;
        this.requiredParametersInjectionResolver = requiredParametersInjectionResolver;
        this.scopeResolver = scopeResolver;
        this.compilationData = compilationData;
    }

    public R<InjectionTree, ImmutableList<InjectionStageError>> Build(Binding binding, CancellationToken cancellationToken)
    {
        var diagnostics = ImmutableList.CreateBuilder<InjectionStageError>();
        var factoryConstructorParameters = ImmutableList.CreateBuilder<FactoryConstructorParameterInjectionNode>();

        var injectionNodePair = this.GetInjectionNode(binding, null, Scope.NewInstance, O.None, factoryConstructorParameters, diagnostics, cancellationToken);
        if (diagnostics.IsEmpty())
        {
            return R.Success(new InjectionTree(injectionNodePair.InjectionNode, injectionNodePair.ImplementDisposable, factoryConstructorParameters.ToImmutable()));
        }

        return R.Error(diagnostics.ToImmutable());
    }

    private (InjectionNode InjectionNode, bool ImplementDisposable) GetInjectionNode(
        Binding binding,
        InjectionNode? parentInjectionNode,
        Scope parentScope,
        O<DefiniteParameter> parameterOption,
        ImmutableList<FactoryConstructorParameterInjectionNode>.Builder factoryMethodParameters,
        ImmutableList<InjectionStageError>.Builder diagnostics,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var constructorParameterCreationNodes = new List<InjectionNode>();

        var scope = this.scopeResolver.ResolveScope(binding);
        var implementIDisposable = binding.ImplementsIDisposable;
        var creationInjectionNode = this.CreateInjectionNode(
           binding.TargetType,
           binding.CommonType,
           scope,
           constructorParameterCreationNodes,
           CreationSource.From(binding.Method),
           parentInjectionNode,
           parentScope,
           implementIDisposable,
           O.From(binding.IsNewOverridable, binding.Method.Parameters),
           O.From(binding.IsInjectable, binding.CommonType).Combine(parameterOption, (type, parameter) => new ParameterNode(type, this.GetParameterSource(type, parameter, diagnostics), parameter.Name, parameter.TypeMetadata, scope == Scope.NewInstance, parentInjectionNode)),
           diagnostics);

        foreach (var parameter in binding.Method.Parameters)
        {
            var resolvedBinding = this.bindingResolver.ResolveBinding(parameter);
            switch (resolvedBinding)
            {
                case SingleParameter singleParameter:
                    {
                        var injectionNodePair = this.GetInjectionNode(singleParameter.Binding, creationInjectionNode, scope, O.Some(parameter), factoryMethodParameters, diagnostics, cancellationToken);
                        BooleanHelper.SetIfTrue(ref implementIDisposable, injectionNodePair.ImplementDisposable);
                        constructorParameterCreationNodes.Add(injectionNodePair.InjectionNode);
                        break;
                    }

                case ArrayParameter arrayParameter:
                    {
                        var arrayScope = this.scopeResolver.ResolveScope(arrayParameter.ArrayType);
                        var arrayConstructorParameterCreationNodes = new List<InjectionNode>();
                        var arrayInjectionNode = this.CreateInjectionNode(arrayParameter.ArrayType, arrayParameter.ArrayType, arrayScope, arrayConstructorParameterCreationNodes, CreationSource.ArrayCreation(arrayParameter.ArrayType.ElementType), creationInjectionNode, arrayScope, false, O.None, O.None, diagnostics);
                        var parameterInjectionNodePairs = arrayParameter.Bindings.Select(x => this.GetInjectionNode(x, arrayInjectionNode, arrayScope, O.Some(parameter), factoryMethodParameters, diagnostics, cancellationToken)).ToArray();
                        arrayConstructorParameterCreationNodes.AddRange(parameterInjectionNodePairs.Select(x => x.InjectionNode));
                        BooleanHelper.SetIfTrue(ref implementIDisposable, parameterInjectionNodePairs.Any(x => x.ImplementDisposable));
                        constructorParameterCreationNodes.Add(arrayInjectionNode);
                        break;
                    }

                case ExternalParameter externalParameter:
                    var externalParameterScope = this.scopeResolver.ResolveScope(externalParameter.Type);
                    constructorParameterCreationNodes.Add(this.CreateParameterInjectionNode(externalParameter.Type, factoryMethodParameters, parameter, creationInjectionNode, externalParameterScope, diagnostics));
                    break;
                case Error error:
                    diagnostics.Add(new InjectionStageError.ResolveTypeError(error.BindingError, parentInjectionNode?.Name ?? Root));
                    break;
            }
        }

        return (creationInjectionNode, implementIDisposable);
    }

    private InjectionNode CreateParameterInjectionNode(
        DefiniteType type,
        ImmutableList<FactoryConstructorParameterInjectionNode>.Builder factoryMethodParameters,
        DefiniteParameter parameter,
        InjectionNode injectionNode,
        Scope scope,
        ImmutableList<InjectionStageError>.Builder diagnostics)
    {
        var parameterSource = this.GetParameterSource(type, parameter, diagnostics);

        if (scope == Scope.SingleInstancePerFactory)
        {
            var factoryConstructorParameterInjectionNode =
                new FactoryConstructorParameterInjectionNode(type, parameter.Name, parameterSource, parameter.TypeMetadata, injectionNode);
            factoryMethodParameters.Add(factoryConstructorParameterInjectionNode);
            return factoryConstructorParameterInjectionNode;
        }

        return InjectionNode.FactoryMethodParameterInjectionNode(type, parameter.Name, parameterSource, parameter.TypeMetadata, !parameter.TypeMetadata.IsValueType && scope == Scope.NewInstance, injectionNode);
    }

    private ParameterSource GetParameterSource(DefiniteType type, DefiniteParameter parameter, ImmutableList<InjectionStageError>.Builder diagnostics)
    {
        var resolveParameterSource = this.requiredParametersInjectionResolver.ResolveParameterSource(type, parameter.Name);
        var parameterSource = ParameterSource.DirectParameter(this.requiredParametersInjectionResolver.Inject);
        switch (resolveParameterSource)
        {
            case ResolvedParameterSource.NoExactMatch noExactMatch:
                diagnostics.Add(
                    new InjectionStageError.ResolveParameterError(
                        noExactMatch.Type,
                        noExactMatch.Name,
                        noExactMatch.ParameterSources));
                break;
            case ResolvedParameterSource.Found success:
                parameterSource = success.ParameterSource;
                break;
        }

        return parameterSource;
    }

    private InjectionNode CreateInjectionNode(
        DefiniteType targetType,
        DefiniteType commonType,
        Scope scope,
        List<InjectionNode> parameterCreationNodes,
        CreationSource creationSource,
        InjectionNode? parentInjectionNode,
        Scope parentScope,
        bool implementsDisposable,
        O<ValueArray<DefiniteParameter>> overridableNewParametersOption,
        O<ParameterNode> parameterNodeOption,
        ImmutableList<InjectionStageError>.Builder diagnostics)
    {
        switch (scope)
        {
            case Scope.AutoScope:
            case Scope.NewInstanceScope:
                if (parentScope == Scope.SingleInstancePerRequest || parentScope == Scope.SingleInstancePerFactory || parentScope is Scope.SingleInstancePerFuncResultScope)
                {
                    diagnostics.Add(
                        new InjectionStageError.ScopeError(
                            targetType,
                            Scope.NewInstance,
                            parentInjectionNode?.Name ?? Root,
                            parentScope.ToString()));
                }

                return InjectionNode.NewInstanceInjectionNode(targetType, commonType, implementsDisposable, parameterCreationNodes, creationSource, parameterNodeOption, overridableNewParametersOption, parentInjectionNode);
            case Scope.SingleInstancePerRequestScope:
                if (parentScope == Scope.SingleInstancePerFactory || parentScope is Scope.SingleInstancePerFuncResultScope)
                {
                    diagnostics.Add(
                        new InjectionStageError.ScopeError(
                            targetType,
                            scope,
                            parentInjectionNode?.Name ?? Root,
                            $"{Scope.SingleInstancePerFactory}, {nameof(Scope.SingleInstancePerFuncResultScope)}"));
                }

                return InjectionNode.SingleInstancePerRequestInjectionNode(targetType, commonType, implementsDisposable, parameterCreationNodes, creationSource, parameterNodeOption, overridableNewParametersOption, parentInjectionNode);
            case Scope.SingleInstancePerFuncResultScope:
                if (parentScope == Scope.SingleInstancePerFactory)
                {
                    diagnostics.Add(new InjectionStageError.ScopeError(
                        targetType,
                        scope,
                        parentInjectionNode?.Name ?? Root,
                        Scope.SingleInstancePerFactory.ToString()));
                }

                return InjectionNode.SingleInstancePerFactoryInjectionNode(targetType, commonType, implementsDisposable, parameterCreationNodes, creationSource, parameterNodeOption, overridableNewParametersOption, parentInjectionNode);
            case Scope.SingleInstancePerFactoryScope:
                return InjectionNode.SingleInstancePerFactoryInjectionNode(targetType, commonType, implementsDisposable, parameterCreationNodes, creationSource, parameterNodeOption, overridableNewParametersOption, parentInjectionNode);
            default:
                throw new UnreachableCaseException(typeof(Scope));
        }
    }
}