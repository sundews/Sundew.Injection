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

internal sealed class InjectionTreeBuilder
{
    private const string Root = "<root>";
    private readonly BindingResolver bindingResolver;
    private readonly ParametersInjectionResolver parametersInjectionResolver;
    private readonly ScopeResolver scopeResolver;

    public InjectionTreeBuilder(
        BindingResolver bindingResolver,
        ParametersInjectionResolver parametersInjectionResolver,
        ScopeResolver scopeResolver)
    {
        this.bindingResolver = bindingResolver;
        this.parametersInjectionResolver = parametersInjectionResolver;
        this.scopeResolver = scopeResolver;
    }

    public R<InjectionTree, ImmutableList<InjectionStageError>> Build(Binding binding, Type returnType, CancellationToken cancellationToken)
    {
        var injectionModelResult = this.GetInjectionModel(binding, default, returnType, null, default, cancellationToken);
        if (injectionModelResult.IsSuccess)
        {
            var injectionModel = injectionModelResult.Value;
            return R.Success(new InjectionTree(injectionModel.InjectionNode, injectionModel.Lifecycle, injectionModel.Lifecycle));
        }

        return R.Error(injectionModelResult.Error);
    }

    private R<InjectionModel, ImmutableList<InjectionStageError>> GetInjectionModel(
        Binding binding,
        InjectionNode? dependantInjectionNode,
        Type requestedType,
        RequestingParameter? requestingParameterOption,
        Type? dependantTypeOption,
        CancellationToken cancellationToken)
    {
        static bool IsReferencedTypeMismatchError(Type parameterType, Binding binding, Scope scope)
        {
            return parameterType == binding.TargetType && binding.ReferencedType != binding.TargetType && scope is not Scope.NewInstance;
        }

        cancellationToken.ThrowIfCancellationRequested();
        var constructorParameterInjectionNodes = new RecordList<InjectionNode>();

        var scope = this.scopeResolver.ResolveScope(binding.ReferencedType);
        if (IsReferencedTypeMismatchError(requestedType, binding, scope))
        {
            return R.Error(ImmutableList.Create(InjectionStageError._ReferencedTypeMismatchError(binding.TargetType, binding.ReferencedType, scope, dependantInjectionNode?.DependantName ?? Root)));
        }

        var lifecycle = binding.Lifecycle;
        var creationResult = this.GetCreationSource(binding, dependantInjectionNode, cancellationToken);
        if (creationResult.TryGetError(out var error, out var creation))
        {
            return R.Error(error);
        }

        var errors = ImmutableList.CreateBuilder<InjectionStageError>();
        errors.AddErrors(creationResult);

        var parameterSourceOption =
            requestingParameterOption.MapValue(parameter =>
                this.GetParameterSource(binding.ReferencedType, parameter.Name, dependantTypeOption, errors).MapValue(
                    parameterSource =>
                        new RequestingParameterAndSource(parameter.Name, parameter.Metadata, parameter.IsOptional, parameterSource)));

        var creationInjectionNode = this.CreateInjectionNode(
            binding.TargetType,
            binding.ReferencedType,
            scope,
            constructorParameterInjectionNodes,
            creation.CreationSource,
            dependantInjectionNode,
            lifecycle,
            binding.IsNewOverridable.ToOption(binding.Method.Parameters),
            parameterSourceOption.Map(parameterPair =>
                    new ParameterNode(
                        binding.ReferencedType,
                        parameterPair.ParameterSource,
                        parameterPair.Name,
                        parameterPair.Metadata,
                        scope is Scope.NewInstance,
                        scope is Scope.SingleInstancePerFactory,
                        dependantInjectionNode?.GetInjectionNodeName(),
                        parameterPair.IsOptional)));
        lifecycle |= creation.Lifecycle;

        foreach (var childParameter in binding.Method.Parameters)
        {
            var resolvedBinding = this.bindingResolver.ResolveBinding(childParameter, binding.TargetType, this.parametersInjectionResolver);
            switch (resolvedBinding)
            {
                case ThisFactoryParameter thisFactoryParameter:
                    constructorParameterInjectionNodes.AddIfHasValue(InjectionNode.ThisFactoryInjectionNode(thisFactoryParameter.FactoryType, creationInjectionNode.GetInjectionNodeName()));
                    break;
                case SingleParameter singleParameter:
                    {
                        var injectionModelResult = this.GetInjectionModel(singleParameter.Binding, creationInjectionNode, childParameter.Type, new RequestingParameter(childParameter.Name, childParameter.TypeMetadata with { Lifecycle = singleParameter.Binding.Lifecycle }, childParameter.ParameterNecessity.IsOptional), binding.TargetType, cancellationToken);

                        errors.AddErrors(injectionModelResult);

                        if (!injectionModelResult.IsSuccess)
                        {
                            break;
                        }

                        var injectionModel = injectionModelResult.Value;
                        lifecycle |= injectionModel.Lifecycle;

                        constructorParameterInjectionNodes.AddIfHasValue(injectionModel.InjectionNode);
                        break;
                    }

                case MultiItemParameter multiItemParameter:
                    {
                        var multiItemScope = this.scopeResolver.ResolveScope(multiItemParameter.Type);
                        var creationSource = multiItemScope is Scope.NewInstance && !multiItemParameter.IsArrayRequired
                                ? CreationSource._IteratorMethodCall(multiItemParameter.Type, multiItemParameter.ElementType)
                                : CreationSource._ArrayCreation(multiItemParameter.ElementType);
                        var arrayConstructorInjectionNodes = new RecordList<InjectionNode>();
                        var arrayInjectionNode = this.CreateInjectionNode(multiItemParameter.Type, multiItemParameter.Type, multiItemScope, arrayConstructorInjectionNodes, creationSource, creationInjectionNode, Lifecycle.None, null, null);

                        var parameterInjectionNodePairs = multiItemParameter.Bindings.Select(x => this.GetInjectionModel(x, arrayInjectionNode, childParameter.Type, new RequestingParameter(childParameter.Name, childParameter.TypeMetadata, childParameter.ParameterNecessity.IsOptional), default, cancellationToken)).ToArray();

                        errors.AddAnyErrors(parameterInjectionNodePairs);

                        var successes = parameterInjectionNodePairs.GetSuccesses().ToReadOnlyCollection();
                        arrayConstructorInjectionNodes.AddRange(successes.Select(x => x.InjectionNode));
                        lifecycle |= successes.Aggregate(Lifecycle.None, (previous, injectionModel) => previous | injectionModel.Lifecycle);

                        constructorParameterInjectionNodes.Add(arrayInjectionNode);
                        break;
                    }

                case OptionalParameter defaultParameter:
                    constructorParameterInjectionNodes.Add(
                        new NewInstanceInjectionNode(
                            childParameter.Type,
                            childParameter.Type,
                            childParameter.Type.IsValueType ? defaultParameter.TypeMetadata.Lifecycle : Lifecycle.None,
                            new RecordList<InjectionNode>(),
                            defaultParameter.Literal != null ? CreationSource._LiteralValue(defaultParameter.Literal.ToString()) : CreationSource._DefaultValue(defaultParameter.Type),
                            null,
                            null,
                            creationInjectionNode.GetInjectionNodeName()));
                    break;

                case RequiredParameter requiredParameter:
                    var requiredParameterScope = this.scopeResolver.ResolveScope(requiredParameter.Type);
                    var requiredParameterInjectionNode = this.CreateParameterInjectionNode(
                        requiredParameter.Type,
                        new NamedParameter(childParameter.Name, childParameter.TypeMetadata, childParameter.DefaultConstructor, childParameter.ParameterNecessity.IsOptional),
                        creationInjectionNode.GetInjectionNodeName(),
                        requiredParameterScope,
                        requiredParameter.ParameterSource);

                    constructorParameterInjectionNodes.Add(requiredParameterInjectionNode);
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

        return R.From(errors.Count == 0, new InjectionModel(creationInjectionNode, binding.IsNewOverridable ? Lifecycle.Both : lifecycle), errors.ToImmutable());
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
                return R.Success(new CreationModel(CreationSource._ConstructorCall(bindingMethod.ContainingType), Lifecycle.None));
            case MethodKind.Static:
                return R.Success(new CreationModel(CreationSource._StaticMethodCall(bindingMethod.ContainingType, bindingMethod), Lifecycle.None));
            case MethodKind.Instance instance:
                var resolvedBinding = this.bindingResolver.ResolveBinding(bindingMethod.ContainingType, instance.ContainingTypeMetadata, instance.ContainingTypeDefaultConstructor, default, default, this.parametersInjectionResolver);
                switch (resolvedBinding)
                {
                    case ThisFactoryParameter thisFactoryParameter:
                        return R.Error(ImmutableList.Create(InjectionStageError._UnsupportedInstanceMethodError(bindingMethod, thisFactoryParameter.FactoryType, Root)));
                    case RequiredParameter requiredParameter:
                        var type = requiredParameter.Type;
                        var requiredExternalParameterScope = this.scopeResolver.ResolveScope(requiredParameter.Type);
                        var requiredExternalInjectionNode = this.CreateParameterInjectionNode(requiredParameter.Type, new NamedParameter(requiredParameter.Type.Name, requiredParameter.TypeMetadata, default), string.Empty, requiredExternalParameterScope, requiredParameter.ParameterSource);
                        return R.Success(new CreationModel(CreationSource._InstanceMethodCall(type, bindingMethod, requiredExternalInjectionNode, instance.IsProperty), Lifecycle.None));
                    case ScopeError scopeError:
                        return R.Error(ImmutableList.Create(InjectionStageError._ScopeError(scopeError.CurrentType, scopeError.CurrentScope, Root, scopeError.Dependant.Scope.ToString())));
                    case SingleParameter singleParameter:
                        var injectionModelResult = this.GetInjectionModel(singleParameter.Binding, dependantInjectionNode, singleParameter.Binding.ReferencedType, new RequestingParameter(singleParameter.Binding.TargetType.Name, Metadata: instance.ContainingTypeMetadata, false), default, cancellationToken);
                        return injectionModelResult.Map(injectionModel =>
                            new CreationModel(CreationSource._InstanceMethodCall(bindingMethod.ContainingType, bindingMethod, injectionModel.InjectionNode, instance.IsProperty), injectionModel.Lifecycle));
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

    private InjectionNode CreateParameterInjectionNode(
        Type type,
        NamedParameter parameter,
        string dependantName,
        Scope scope,
        ParameterSource parameterSource)
    {
        if (scope is Scope.SingleInstancePerFactory)
        {
            var factoryConstructorParameterInjectionNode =
                new FactoryConstructorParameterInjectionNode(type, parameter.Name, parameterSource, parameter.Metadata, dependantName, parameter.IsTargetOptional);
            return factoryConstructorParameterInjectionNode;
        }

        return InjectionNode.FactoryMethodParameterInjectionNode(type, parameter.Name, parameterSource, parameter.Metadata, dependantName, parameter.IsTargetOptional);
    }

    private ParameterSource? GetParameterSource(Type type, string parameterName, Type? dependantTypeOption, ImmutableList<InjectionStageError>.Builder diagnostics)
    {
        var resolveParameterSource = this.parametersInjectionResolver.ResolveParameterSource(type, parameterName, dependantTypeOption);
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
                return success.ParameterSource;
            case ResolvedParameterSource.NotFound:
                break;
        }

        return default;
    }

    private InjectionNode CreateInjectionNode(
        Type targetType,
        Type referencedType,
        Scope scope,
        RecordList<InjectionNode> parameterCreationNodes,
        CreationSource creationSource,
        InjectionNode? dependantInjectionNode,
        Lifecycle lifecycle,
        ValueArray<FullParameter>? overridableNewParametersOption,
        ParameterNode? parameterNodeOption)
    {
        return scope switch
        {
            Scope.Auto =>
                InjectionNode.NewInstanceInjectionNode(
                    targetType,
                    referencedType,
                    lifecycle,
                    parameterCreationNodes,
                    creationSource,
                    parameterNodeOption,
                    overridableNewParametersOption,
                    dependantInjectionNode?.GetInjectionNodeName()),
            Scope.NewInstance =>
               InjectionNode.NewInstanceInjectionNode(
                   targetType,
                   referencedType,
                   lifecycle,
                   parameterCreationNodes,
                   creationSource,
                   parameterNodeOption,
                   overridableNewParametersOption,
                   dependantInjectionNode?.GetInjectionNodeName()),
            Scope.SingleInstancePerRequest =>
                InjectionNode.SingleInstancePerRequestInjectionNode(
                    targetType,
                    referencedType,
                    lifecycle,
                    parameterCreationNodes,
                    creationSource,
                    parameterNodeOption,
                    overridableNewParametersOption,
                    dependantInjectionNode?.GetInjectionNodeName()),
            Scope.SingleInstancePerFuncResult =>
                InjectionNode.SingleInstancePerFactoryInjectionNode(
                    targetType,
                    referencedType,
                    lifecycle,
                    parameterCreationNodes,
                    creationSource,
                    parameterNodeOption,
                    overridableNewParametersOption,
                    dependantInjectionNode?.GetInjectionNodeName()),
            Scope.SingleInstancePerFactory singleInstancePerFactory =>
                 InjectionNode.SingleInstancePerFactoryInjectionNode(
                     targetType,
                     referencedType,
                     lifecycle,
                     parameterCreationNodes,
                     creationSource,
                     parameterNodeOption,
                     overridableNewParametersOption,
                     dependantInjectionNode?.GetInjectionNodeName()),
        };
    }

    private readonly record struct CreationModel(CreationSource CreationSource, Lifecycle Lifecycle);

    private readonly record struct InjectionModel(
        InjectionNode InjectionNode,
        Lifecycle Lifecycle);

    private readonly record struct RequestingParameter(string Name, TypeMetadata Metadata, bool IsOptional);

    private readonly record struct RequestingParameterAndSource(string Name, TypeMetadata Metadata, bool IsOptional, ParameterSource ParameterSource);
}