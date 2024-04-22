// --------------------------------------------------------------------------------------------------------------------
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
using Sundew.DiscriminatedUnions;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Extensions;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using CreateGenericMethodError = Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers.CreateGenericMethodError;
using MethodKind = Sundew.Injection.Generator.TypeSystem.MethodKind;
using NewInstanceInjectionNode = Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes.NewInstanceInjectionNode;
using ParameterNode = Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes.ParameterNode;
using Scope = Sundew.Injection.Generator.TypeSystem.Scope;

internal sealed class InjectionTreeBuilder(
    BindingResolver bindingResolver,
    RequiredParametersInjectionResolver requiredParametersInjectionResolver,
    ScopeResolver scopeResolver)
{
    private const string Root = "<root>";

    public R<InjectionTree, ImmutableList<InjectionStageError>> Build(Binding binding, CancellationToken cancellationToken)
    {
        var injectionModelResult = this.GetInjectionModel(binding, null, null, cancellationToken);
        if (injectionModelResult.IsSuccess)
        {
            var injectionModel = injectionModelResult.Value;
            return R.Success(new InjectionTree(injectionModel.InjectionNode, injectionModel.FactoryConstructorParameters, injectionModel.NeedsLifecycleHandling, injectionModel.NeedsLifecycleHandling));
        }

        return R.Error(injectionModelResult.Error);
    }

    private R<InjectionModel, ImmutableList<InjectionStageError>> GetInjectionModel(
        Binding binding,
        InjectionNode? dependantInjectionNode,
        (Type Type, string Name, TypeMetadata TypeMetadata)? parameterOption,
        CancellationToken cancellationToken)
    {
        var factoryConstructorParameters = ImmutableList.CreateBuilder<FactoryConstructorParameter>();
        cancellationToken.ThrowIfCancellationRequested();
        var constructorParameterCreationNodes = new RecordList<InjectionNode>();

        var scope = scopeResolver.ResolveScope(binding.ReferencedType);
        var needsLifecycleHandling = binding.HasLifecycle | binding.IsNewOverridable;
        var creationResult = this.GetCreationSource(binding, dependantInjectionNode, cancellationToken);
        if (!creationResult.IsSuccess)
        {
            return R.Error(creationResult.Error);
        }

        var errors = ImmutableList.CreateBuilder<InjectionStageError>();
        if (creationResult.IsError)
        {
            errors.AddRange(creationResult.Error);
        }

        var creation = creationResult.Value;
        factoryConstructorParameters.AddRange(creation.FactoryConstructorParameters);

        BooleanHelper.SetIfTrue(ref needsLifecycleHandling, creation.NeedsLifecycleHandling);

        var creationInjectionNode = this.CreateInjectionNode(
            binding.TargetType,
            binding.ReferencedType,
            scope,
            constructorParameterCreationNodes,
            creation.CreationSource,
            dependantInjectionNode,
            needsLifecycleHandling,
            binding.IsNewOverridable.ToOption(binding.Method.Parameters),
            binding.IsInjectable.ToOption(binding.ReferencedType)
                .Combine(parameterOption, (targetReferenceType, parameter) =>
                    new ParameterNode(
                        targetReferenceType,
                        this.GetParameterSource(targetReferenceType, parameter.Name, errors),
                        parameter.Name,
                        parameter.TypeMetadata,
                        scope is Scope.NewInstance,
                        scope is Scope.SingleInstancePerFactory,
                        dependantInjectionNode?.GetInjectionNodeName())));

        foreach (var parameter in binding.Method.Parameters)
        {
            var resolvedBinding = bindingResolver.ResolveBinding(parameter);
            switch (resolvedBinding)
            {
                case ThisFactoryParameter thisFactoryParameter:
                    constructorParameterCreationNodes.AddIfHasValue(InjectionNode.ThisFactoryInjectionNode(thisFactoryParameter.FactoryType, creationInjectionNode.GetInjectionNodeName()));
                    break;
                case SingleParameter singleParameter:
                    {
                        var injectionModelResult = this.GetInjectionModel(singleParameter.Binding, creationInjectionNode, (parameter.Type, parameter.Name, parameter.TypeMetadata), cancellationToken);

                        errors.AddErrors(injectionModelResult);

                        if (!injectionModelResult.IsSuccess)
                        {
                            break;
                        }

                        var injectionModel = injectionModelResult.Value;
                        BooleanHelper.SetIfTrue(ref needsLifecycleHandling, injectionModel.NeedsLifecycleHandling);
                        factoryConstructorParameters.AddRange(injectionModel.FactoryConstructorParameters);
                        constructorParameterCreationNodes.AddIfHasValue(injectionModel.InjectionNode);
                        break;
                    }

                case MultiItemParameter multiItemParameter:
                    {
                        var multiItemScope = scopeResolver.ResolveScope(multiItemParameter.Type);
                        var creationSource = multiItemScope is Scope.NewInstance && !multiItemParameter.IsArrayRequired
                                ? CreationSource._IteratorMethodCall(multiItemParameter.Type, multiItemParameter.ElementType)
                                : CreationSource._ArrayCreation(multiItemParameter.ElementType);
                        var arrayConstructorInjectionNodes = new RecordList<InjectionNode>();
                        var arrayInjectionNode = this.CreateInjectionNode(multiItemParameter.Type, multiItemParameter.Type, multiItemScope, arrayConstructorInjectionNodes, creationSource, creationInjectionNode, false, null, null);

                        var parameterInjectionNodePairs = multiItemParameter.Bindings.Select(x => this.GetInjectionModel(x, arrayInjectionNode, (parameter.Type, parameter.Name, parameter.TypeMetadata), cancellationToken)).ToArray();

                        errors.AddAnyErrors(parameterInjectionNodePairs);

                        var successes = parameterInjectionNodePairs.GetSuccesses().ToReadOnly();
                        arrayConstructorInjectionNodes.AddRange(successes.Select(x => x.InjectionNode));
                        BooleanHelper.SetIfTrue(ref needsLifecycleHandling, successes.Any(x => x.NeedsLifecycleHandling));

                        constructorParameterCreationNodes.Add(arrayInjectionNode);
                        break;
                    }

                case OptionalParameter defaultParameter:
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

                case RequiredParameter externalParameter:
                    var externalParameterScope = scopeResolver.ResolveScope(externalParameter.Type);
                    var externalParameterInjectionNode = this.CreateParameterInjectionNode(
                        externalParameter.Type,
                        new NamedParameter(parameter.Name, parameter.TypeMetadata, parameter.DefaultConstructor),
                        creationInjectionNode.GetInjectionNodeName(),
                        externalParameterScope,
                        externalParameter.ParameterSource);
                    factoryConstructorParameters.AddIfHasValue(externalParameterInjectionNode.FactoryConstructorParameterOption);

                    constructorParameterCreationNodes.Add(externalParameterInjectionNode.InjectionNode);
                    break;
                case ScopeError scopeError:
                    errors.Add(InjectionStageError._ScopeError(scopeError.CurrentType, scopeError.CurrentScope, dependantInjectionNode?.GetInjectionNodeName() ?? Root, scopeError.Dependant.Scope.ToString()));
                    break;
                case ParameterError parameterError:
                    errors.Add(InjectionStageError._ResolveParameterError(parameterError.Type, dependantInjectionNode?.GetInjectionNodeName() ?? Root, parameterError.ParameterSources));
                    break;
                case CreateGenericMethodError bindingError:
                    errors.Add(InjectionStageError._CreateGenericMethodError(bindingError.Error, dependantInjectionNode?.GetInjectionNodeName() ?? Root));
                    break;
            }
        }

        return R.From(true, new InjectionModel(creationInjectionNode, needsLifecycleHandling, factoryConstructorParameters.ToImmutable()), errors.ToImmutable());
    }

    private R<CreationModel, ImmutableList<InjectionStageError>> GetCreationSource(
        Binding binding,
        InjectionNode? dependantInjectionNode,
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
                var resolvedBinding = bindingResolver.ResolveBinding(bindingMethod.ContainingType, instance.ContainingTypeMetadata, instance.ContainingTypeDefaultConstructor, null);
                switch (resolvedBinding)
                {
                    case ThisFactoryParameter thisFactoryParameter:
                        return R.Error(ImmutableList.Create(InjectionStageError._UnsupportedInstanceMethodError(bindingMethod, thisFactoryParameter.FactoryType, Root)));
                    case RequiredParameter externalParameter:
                        var type = externalParameter.Type;
                        var requiredExternalParameterScope = scopeResolver.ResolveScope(externalParameter.Type);
                        var requiredExternalInjectionNode = this.CreateParameterInjectionNode(externalParameter.Type, new NamedParameter(externalParameter.Type.Name, externalParameter.TypeMetadata, default), string.Empty, requiredExternalParameterScope, externalParameter.ParameterSource);
                        return R.Success(new CreationModel(CreationSource._InstanceMethodCall(type, bindingMethod, requiredExternalInjectionNode.InjectionNode, instance.IsProperty), false, ImmutableList<FactoryConstructorParameter>.Empty.AddIfHasValue(requiredExternalInjectionNode.FactoryConstructorParameterOption)));
                    case ScopeError scopeError:
                        return R.Error(ImmutableList.Create(InjectionStageError._ScopeError(scopeError.CurrentType, scopeError.CurrentScope, Root, scopeError.Dependant.Scope.ToString())));
                    case SingleParameter singleParameter:
                        var injectionModelResult = this.GetInjectionModel(singleParameter.Binding, dependantInjectionNode, (singleParameter.Binding.TargetType, singleParameter.Binding.TargetType.Name, TypeMetadata: instance.ContainingTypeMetadata), cancellationToken);
                        return injectionModelResult.With(injectionModel =>
                            new CreationModel(CreationSource._InstanceMethodCall(bindingMethod.ContainingType, bindingMethod, injectionModel.InjectionNode, instance.IsProperty), injectionModel.NeedsLifecycleHandling, injectionModel.FactoryConstructorParameters));
                    case MultiItemParameter multiItemParameter:
                        return R.Error(ImmutableList.Create(InjectionStageError._UnsupportedInstanceMethodError(bindingMethod, multiItemParameter.Type, Root)));
                    case OptionalParameter:
                        return R.Error(ImmutableList.Create(InjectionStageError._UnsupportedInstanceMethodError(bindingMethod, bindingMethod.ContainingType, Root)));
                    case ParameterError parameterError:
                        return R.Error(ImmutableList.Create(InjectionStageError._ResolveParameterError(parameterError.Type, Root, parameterError.ParameterSources)));
                    case CreateGenericMethodError createGenericMethodError:
                        return R.Error(ImmutableList.Create(InjectionStageError._CreateGenericMethodError(createGenericMethodError.Error, Root)));
                }

                break;
        }

        throw new UnreachableCaseException(typeof(MethodKind));
    }

    private (InjectionNode InjectionNode, FactoryConstructorParameter? FactoryConstructorParameterOption) CreateParameterInjectionNode(
        Type type,
        NamedParameter parameter,
        string dependantName,
        Scope scope,
        ParameterSource parameterSource)
    {
        if (scope is Scope.SingleInstancePerFactory)
        {
            var factoryConstructorParameterInjectionNode =
                new FactoryConstructorParameterInjectionNode(type, parameter.Name, parameterSource, parameter.Metadata, dependantName);
            return (factoryConstructorParameterInjectionNode, new FactoryConstructorParameter(type, parameter.Name, parameter.Metadata));
        }

        return (InjectionNode.FactoryMethodParameterInjectionNode(type, parameter.Name, parameterSource, parameter.Metadata, !type.IsValueType && scope is Scope.NewInstance, dependantName), default);
    }

    private ParameterSource GetParameterSource(Type type, string parameterName, ImmutableList<InjectionStageError>.Builder diagnostics)
    {
        var resolveParameterSource = requiredParametersInjectionResolver.ResolveParameterSource(type, parameterName);
        var parameterSource = ParameterSource.DirectParameter(requiredParametersInjectionResolver.Inject);
        switch (resolveParameterSource)
        {
            case ResolvedParameterSource.NoExactMatch noExactMatch:
                diagnostics.Add(
                    InjectionStageError._ResolveParameterError(
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

    private InjectionNode CreateInjectionNode(
        Type targetType,
        Type referencedType,
        Scope scope,
        RecordList<InjectionNode> parameterCreationNodes,
        CreationSource creationSource,
        InjectionNode? dependantInjectionNode,
        bool needsLifecycleHandling,
        ValueArray<FullParameter>? overridableNewParametersOption,
        ParameterNode? parameterNodeOption)
    {
        return scope switch
        {
            Scope.Auto =>
                InjectionNode.NewInstanceInjectionNode(
                    targetType,
                    referencedType,
                    needsLifecycleHandling,
                    parameterCreationNodes,
                    creationSource,
                    parameterNodeOption,
                    overridableNewParametersOption,
                    dependantInjectionNode?.GetInjectionNodeName()),
            Scope.NewInstance =>
               InjectionNode.NewInstanceInjectionNode(
                   targetType,
                   referencedType,
                   needsLifecycleHandling,
                   parameterCreationNodes,
                   creationSource,
                   parameterNodeOption,
                   overridableNewParametersOption,
                   dependantInjectionNode?.GetInjectionNodeName()),
            Scope.SingleInstancePerRequest =>
                InjectionNode.SingleInstancePerRequestInjectionNode(
                    targetType,
                    referencedType,
                    needsLifecycleHandling,
                    parameterCreationNodes,
                    creationSource,
                    parameterNodeOption,
                    overridableNewParametersOption,
                    dependantInjectionNode?.GetInjectionNodeName()),
            Scope.SingleInstancePerFuncResult =>
                InjectionNode.SingleInstancePerFactoryInjectionNode(
                    targetType,
                    referencedType,
                    needsLifecycleHandling,
                    parameterCreationNodes,
                    creationSource,
                    parameterNodeOption,
                    overridableNewParametersOption,
                    dependantInjectionNode?.GetInjectionNodeName()),
            Scope.SingleInstancePerFactory singleInstancePerFactory =>
                 InjectionNode.SingleInstancePerFactoryInjectionNode(
                     targetType,
                     referencedType,
                     needsLifecycleHandling,
                     parameterCreationNodes,
                     creationSource,
                     parameterNodeOption,
                     overridableNewParametersOption,
                     dependantInjectionNode?.GetInjectionNodeName()),
        };
    }

    private readonly record struct CreationModel(CreationSource CreationSource, bool NeedsLifecycleHandling, ImmutableList<FactoryConstructorParameter> FactoryConstructorParameters);

    private readonly record struct InjectionModel(
        InjectionNode InjectionNode,
        bool NeedsLifecycleHandling,
        ImmutableList<FactoryConstructorParameter> FactoryConstructorParameters);
}