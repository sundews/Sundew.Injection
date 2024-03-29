﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionTreeBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Sundew.Base;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;
using Sundew.DiscriminatedUnions;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Extensions;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using MethodKind = Sundew.Injection.Generator.TypeSystem.MethodKind;
using NewInstanceInjectionNode = Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes.NewInstanceInjectionNode;
using ParameterNode = Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes.ParameterNode;
using Scope = Sundew.Injection.Generator.TypeSystem.Scope;

internal sealed class InjectionTreeBuilder
{
    private const string Root = "<root>";
    private readonly BindingResolver bindingResolver;
    private readonly RequiredParametersInjectionResolver requiredParametersInjectionResolver;
    private readonly ScopeResolver scopeResolver;

    public InjectionTreeBuilder(
        BindingResolver bindingResolver,
        RequiredParametersInjectionResolver requiredParametersInjectionResolver,
        ScopeResolver scopeResolver)
    {
        this.bindingResolver = bindingResolver;
        this.requiredParametersInjectionResolver = requiredParametersInjectionResolver;
        this.scopeResolver = scopeResolver;
    }

    public R<InjectionTree, ImmutableList<InjectionStageError>> Build(Binding binding, CancellationToken cancellationToken)
    {
        var injectionModelResult = this.GetInjectionModel(binding, null, binding.Scope, null, cancellationToken);
        if (injectionModelResult.IsSuccess)
        {
            var injectionModel = injectionModelResult.Value;
            return R.Success(new InjectionTree(injectionModel.InjectionNode, injectionModel.FactoryConstructorParameters, injectionModel.NeedsLifecycleHandling, injectionModel.InjectionNode is not SingleInstancePerFactoryInjectionNode && injectionModel.NeedsLifecycleHandling));
        }

        return R.Error(injectionModelResult.Error);
    }

    private R<InjectionModel, ImmutableList<InjectionStageError>> GetInjectionModel(
        Binding binding,
        InjectionNode? dependeeInjectionNode,
        Scope dependeeScope,
        (DefiniteType Type, string Name, TypeMetadata TypeMetadata)? parameterOption,
        CancellationToken cancellationToken)
    {
        var factoryConstructorParameters = ImmutableList.CreateBuilder<FactoryConstructorParameter>();
        cancellationToken.ThrowIfCancellationRequested();
        var constructorParameterCreationNodes = new RecordList<InjectionNode>();

        var scope = this.scopeResolver.ResolveScope(binding);
        var needsLifecycleHandling = binding.HasLifecycle | binding.IsNewOverridable;
        var creationResult = this.GetCreationSource(binding, dependeeInjectionNode, cancellationToken);
        if (!creationResult.IsSuccess)
        {
            return R.Error(creationResult.Error);
        }

        var errors = ImmutableList.CreateBuilder<InjectionStageError>();
        if (creationResult.HasError)
        {
            errors.AddRange(creationResult.Error);
        }

        var creation = creationResult.Value;
        factoryConstructorParameters.AddRange(creation.FactoryConstructorParameters);

        BooleanHelper.SetIfTrue(ref needsLifecycleHandling, creation.NeedsLifecycleHandling);

        var (creationInjectionNode, scopeError) = this.CreateInjectionNode(
            binding.TargetType,
            binding.TargetReferenceType,
            scope,
            constructorParameterCreationNodes,
            creation.CreationSource,
            dependeeInjectionNode,
            dependeeScope,
            needsLifecycleHandling,
            binding.IsNewOverridable.ToOption(binding.Method.Parameters),
            binding.IsInjectable.ToOption(binding.TargetReferenceType).Combine(parameterOption, (targetReferenceType, parameter) => new ParameterNode(targetReferenceType, this.GetParameterSource(targetReferenceType, parameter.Name, errors), parameter.Name, parameter.TypeMetadata, scope == Scope._NewInstance, scope == Scope._SingleInstancePerFactory, dependeeInjectionNode?.GetInjectionNodeName())));

        errors.TryAdd(scopeError);

        foreach (var parameter in binding.Method.Parameters)
        {
            var resolvedBinding = this.bindingResolver.ResolveBinding(parameter);
            switch (resolvedBinding)
            {
                case SingleParameter singleParameter:
                    {
                        var injectionModelResult = this.GetInjectionModel(singleParameter.Binding, creationInjectionNode, scope, (parameter.Type, parameter.Name, parameter.TypeMetadata), cancellationToken);

                        errors.TryAddErrors(injectionModelResult);

                        if (!injectionModelResult.IsSuccess)
                        {
                            break;
                        }

                        var injectionModel = injectionModelResult.Value;
                        BooleanHelper.SetIfTrue(ref needsLifecycleHandling, injectionModel.NeedsLifecycleHandling);
                        factoryConstructorParameters.AddRange(injectionModel.FactoryConstructorParameters);
                        constructorParameterCreationNodes.TryAdd(injectionModel.InjectionNode);
                        break;
                    }

                case MultiItemParameter multiItemParameter:
                    {
                        var multiItemScope = this.scopeResolver.ResolveScope(multiItemParameter.Type);
                        var creationSource = multiItemScope == Scope._NewInstance && !multiItemParameter.IsArrayRequired
                            ? CreationSource._IteratorMethodCall(multiItemParameter.Type, multiItemParameter.ElementType)
                            : CreationSource._ArrayCreation(multiItemParameter.ElementType);
                        var arrayConstructorInjectionNodes = new RecordList<InjectionNode>();
                        var (arrayInjectionNode, arrayScopeError) = this.CreateInjectionNode(multiItemParameter.Type, multiItemParameter.Type, multiItemScope, arrayConstructorInjectionNodes, creationSource, creationInjectionNode, multiItemScope, false, null, null);
                        errors.TryAdd(arrayScopeError);

                        var parameterInjectionNodePairs = multiItemParameter.Bindings.Select(x => this.GetInjectionModel(x, arrayInjectionNode, multiItemScope, (parameter.Type, parameter.Name, parameter.TypeMetadata), cancellationToken)).ToArray();

                        errors.TryAddAnyErrors(parameterInjectionNodePairs);

                        var successes = parameterInjectionNodePairs.GetSuccesses().ToReadOnly();
                        arrayConstructorInjectionNodes.AddRange(successes.Select(x => x.InjectionNode));
                        BooleanHelper.SetIfTrue(ref needsLifecycleHandling, successes.Any(x => x.NeedsLifecycleHandling));

                        constructorParameterCreationNodes.Add(arrayInjectionNode);
                        break;
                    }

                case DefaultParameter defaultParameter:
                    constructorParameterCreationNodes.Add(
                        new NewInstanceInjectionNode(
                            parameter.Type,
                            parameter.Type,
                            defaultParameter.TypeMetadata.HasLifetime && parameter.Type.IsValueType,
                            new RecordList<InjectionNode>(),
                            defaultParameter.Literal != null ? CreationSource._LiteralValue(defaultParameter.Literal.ToString()) : CreationSource._DefaultValue(defaultParameter.Type),
                            null,
                            null,
                            creationInjectionNode.GetInjectionNodeName()));
                    break;

                case ExternalParameter externalParameter:
                    var externalParameterScope = this.scopeResolver.ResolveScope(externalParameter.Type);
                    var externalParameterInjectionNode = this.CreateParameterInjectionNode(
                        externalParameter.Type,
                        (parameter.Name, parameter.TypeMetadata),
                        creationInjectionNode.GetInjectionNodeName(),
                        externalParameterScope,
                        externalParameter.ParameterSource);
                    factoryConstructorParameters.TryAdd(externalParameterInjectionNode.FactoryConstructorParameterOption);

                    constructorParameterCreationNodes.Add(externalParameterInjectionNode.InjectionNode);
                    break;
                case ResolvedBindingError.ParameterError parameterError:
                    errors.Add(new InjectionStageError.ResolveParameterError(parameterError.Type, dependeeInjectionNode?.GetInjectionNodeName() ?? Root, parameterError.ParameterSources));
                    break;
                case ResolvedBindingError.Error error:
                    errors.Add(new InjectionStageError.ResolveTypeError(error.BindingError, dependeeInjectionNode?.GetInjectionNodeName() ?? Root));
                    break;
            }
        }

        return R.From(true, new InjectionModel(creationInjectionNode, needsLifecycleHandling, factoryConstructorParameters.ToImmutable()), errors.ToImmutable());
    }

    private R<CreationModel, ImmutableList<InjectionStageError>> GetCreationSource(
        Binding binding,
        InjectionNode? dependeeInjectionNode,
        CancellationToken cancellationToken)
    {
        var bindingMethod = binding.Method;
        switch (bindingMethod.Kind)
        {
            case MethodKind.Constructor:
                return R.Success(new CreationModel(CreationSource._ConstructorCall(bindingMethod.ContainingType), false, ImmutableList<FactoryConstructorParameter>.Empty));
            case MethodKind.Static:
                return R.Success(new CreationModel(CreationSource._StaticMethodCall(bindingMethod.ContainingType, bindingMethod), false, ImmutableList<FactoryConstructorParameter>.Empty));
            case MethodKind.Instance instance:
                var resolvedBinding = this.bindingResolver.ResolveBinding(bindingMethod.ContainingType, instance.ContainingTypeMetadata, null);
                switch (resolvedBinding)
                {
                    case ExternalParameter externalParameter:
                        var type = externalParameter.Type;

                        var requiredExternalParameterScope = this.scopeResolver.ResolveScope(externalParameter.Type);
                        var requiredExternalInjectionNode = this.CreateParameterInjectionNode(externalParameter.Type, (externalParameter.Type.Name, externalParameter.TypeMetadata), string.Empty, requiredExternalParameterScope, externalParameter.ParameterSource);
                        return R.Success(new CreationModel(CreationSource._InstanceMethodCall(type, bindingMethod, requiredExternalInjectionNode.InjectionNode, instance.IsProperty), false, ImmutableList<FactoryConstructorParameter>.Empty.TryAdd(requiredExternalInjectionNode.FactoryConstructorParameterOption)));
                    case SingleParameter singleParameter:
                        var factoryScope = this.scopeResolver.ResolveScope(singleParameter.Binding);
                        var injectionModelResult = this.GetInjectionModel(singleParameter.Binding, dependeeInjectionNode, factoryScope, (singleParameter.Binding.TargetType, singleParameter.Binding.TargetType.Name, TypeMetadata: instance.ContainingTypeMetadata), cancellationToken);
                        return injectionModelResult.With(injectionModel =>
                            new CreationModel(CreationSource._InstanceMethodCall(bindingMethod.ContainingType, bindingMethod, injectionModel.InjectionNode, instance.IsProperty), injectionModel.NeedsLifecycleHandling, injectionModel.FactoryConstructorParameters));
                    case MultiItemParameter multiItemParameter:
                        return R.Error(ImmutableList.Create(InjectionStageError._UnsupportedInstanceMethod(bindingMethod, multiItemParameter.Type, Root)));
                    case DefaultParameter:
                        return R.Error(ImmutableList.Create(InjectionStageError._UnsupportedInstanceMethod(bindingMethod, bindingMethod.ContainingType, Root)));
                    case ResolvedBindingError.Error error:
                        return R.Error(ImmutableList.Create(InjectionStageError._ResolveTypeError(error.BindingError, Root)));
                    case ResolvedBindingError.ParameterError parameterError:
                        return R.Error(ImmutableList.Create(InjectionStageError._ResolveParameterError(parameterError.Type, Root, parameterError.ParameterSources)));
                }

                break;
        }

        throw new UnreachableCaseException(typeof(MethodKind));
    }

    private (InjectionNode InjectionNode, FactoryConstructorParameter? FactoryConstructorParameterOption) CreateParameterInjectionNode(
        DefiniteType type,
        (string Name, TypeMetadata TypeMetadata) parameter,
        string dependeeName,
        Scope scope,
        ParameterSource parameterSource)
    {
        if (scope == Scope._SingleInstancePerFactory)
        {
            var factoryConstructorParameterInjectionNode =
                new FactoryConstructorParameterInjectionNode(type, parameter.Name, parameterSource, parameter.TypeMetadata, dependeeName);
            return (factoryConstructorParameterInjectionNode, new FactoryConstructorParameter(type, parameter.Name, parameter.TypeMetadata));
        }

        return (InjectionNode.FactoryMethodParameterInjectionNode(type, parameter.Name, parameterSource, parameter.TypeMetadata, !type.IsValueType && scope == Scope._NewInstance, dependeeName), default);
    }

    private ParameterSource GetParameterSource(DefiniteType type, string parameterName, ImmutableList<InjectionStageError>.Builder diagnostics)
    {
        var resolveParameterSource = this.requiredParametersInjectionResolver.ResolveParameterSource(type, parameterName);
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
            case ResolvedParameterSource.NotFound:
                break;
        }

        return parameterSource;
    }

    private (InjectionNode InjectionNode, R<InjectionStageError> ScopeError) CreateInjectionNode(
        DefiniteType targetType,
        DefiniteType targetReferenceType,
        Scope scope,
        RecordList<InjectionNode> parameterCreationNodes,
        CreationSource creationSource,
        InjectionNode? dependeeInjectionNode,
        Scope dependeeScope,
        bool needsLifecycleHandling,
        ValueArray<DefiniteParameter>? overridableNewParametersOption,
        ParameterNode? parameterNodeOption)
    {
        return scope switch
        {
            Scope.Auto => (
                InjectionNode.NewInstanceInjectionNode(
                    targetType,
                    targetReferenceType,
                    needsLifecycleHandling,
                    parameterCreationNodes,
                    creationSource,
                    parameterNodeOption,
                    overridableNewParametersOption,
                    dependeeInjectionNode?.GetInjectionNodeName()),
                R.From(
                    dependeeScope != Scope._SingleInstancePerRequest &&
                    dependeeScope != Scope._SingleInstancePerFactory &&
                    dependeeScope is not Scope.SingleInstancePerFuncResult,
                    () => InjectionStageError._ScopeError(targetType, Scope._NewInstance, dependeeInjectionNode?.GetInjectionNodeName() ?? Root, dependeeScope.ToString()))),
            Scope.NewInstance => (
                InjectionNode.NewInstanceInjectionNode(
                    targetType,
                    targetReferenceType,
                    needsLifecycleHandling,
                    parameterCreationNodes,
                    creationSource,
                    parameterNodeOption,
                    overridableNewParametersOption,
                    dependeeInjectionNode?.GetInjectionNodeName()),
                R.From(
                    dependeeScope != Scope._SingleInstancePerRequest &&
                    dependeeScope != Scope._SingleInstancePerFactory &&
                    dependeeScope is not Scope.SingleInstancePerFuncResult,
                    () => InjectionStageError._ScopeError(targetType, Scope._NewInstance, dependeeInjectionNode?.GetInjectionNodeName() ?? Root, dependeeScope.ToString()))),
            Scope.SingleInstancePerRequest => (
                InjectionNode.SingleInstancePerRequestInjectionNode(
                    targetType,
                    targetReferenceType,
                    needsLifecycleHandling,
                    parameterCreationNodes,
                    creationSource,
                    parameterNodeOption,
                    overridableNewParametersOption,
                    dependeeInjectionNode?.GetInjectionNodeName()),
                R.From(
                    dependeeScope != Scope._SingleInstancePerFactory &&
                    dependeeScope is not Scope.SingleInstancePerFuncResult,
                    () => InjectionStageError._ScopeError(targetType, scope, dependeeInjectionNode?.GetInjectionNodeName() ?? Root, $"{Scope._SingleInstancePerFactory}, {nameof(Scope.SingleInstancePerFuncResult)}"))),
            Scope.SingleInstancePerFuncResult => (
                InjectionNode.SingleInstancePerFactoryInjectionNode(
                    targetType,
                    targetReferenceType,
                    needsLifecycleHandling,
                    parameterCreationNodes,
                    creationSource,
                    parameterNodeOption,
                    overridableNewParametersOption,
                    dependeeInjectionNode?.GetInjectionNodeName()),
                R.From(
                    dependeeScope == Scope._SingleInstancePerFactory,
                    () => InjectionStageError._ScopeError(targetType, scope, dependeeInjectionNode?.GetInjectionNodeName() ?? Root, Scope._SingleInstancePerFactory.ToString()))),
            Scope.SingleInstancePerFactory => (
                InjectionNode.SingleInstancePerFactoryInjectionNode(
                    targetType,
                    targetReferenceType,
                    needsLifecycleHandling,
                    parameterCreationNodes,
                    creationSource,
                    parameterNodeOption,
                    overridableNewParametersOption,
                    dependeeInjectionNode?.GetInjectionNodeName()),
                R.Success()),
        };
    }

    private readonly record struct CreationModel(CreationSource CreationSource, bool NeedsLifecycleHandling, ImmutableList<FactoryConstructorParameter> FactoryConstructorParameters);

    private readonly record struct InjectionModel(
        InjectionNode InjectionNode,
        bool NeedsLifecycleHandling,
        ImmutableList<FactoryConstructorParameter> FactoryConstructorParameters);
}