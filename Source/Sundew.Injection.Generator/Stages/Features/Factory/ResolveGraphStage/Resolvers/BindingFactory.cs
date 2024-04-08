// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingFactory.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using MethodKind = Sundew.Injection.Generator.TypeSystem.MethodKind;

internal class BindingFactory(
    MethodFactory methodFactory,
    NameRegistry<NamedType> nameTypeRegistrar,
    ITypeRegistrar<ResolvedBinding> resolvedBindingTypeRegistrar,
    ITypeRegistrar<Binding[]> bindingsTypeRegistrar,
    KnownEnumerableTypes knownEnumerableTypes)
{
    public void RegisterThisFactory(NamedType factoryType, NamedType? factoryInterfaceType)
    {
        resolvedBindingTypeRegistrar.Register(
            factoryType.Id,
            factoryInterfaceType?.Id,
            ResolvedBinding.ThisFactoryParameter(factoryType, factoryInterfaceType),
            false);
    }

    public ResolvedBinding TryCreateSingleParameter(BindingRegistration bindingRegistration, Type? returnType = default)
    {
        var targetType = bindingRegistration.TargetType.Type;
        var newBinding = new Binding(
            bindingRegistration.TargetType.Type,
            bindingRegistration.ReferencedType,
            bindingRegistration.Scope,
            bindingRegistration.Method,
            bindingRegistration.TargetType.TypeMetadata.HasLifetime,
            bindingRegistration.IsInjectable,
            bindingRegistration.IsNewOverridable);
        var newResolvedBinding = ResolvedBinding.SingleParameter(newBinding);
        var returnTypeId = returnType?.Id;
        var targetTypeId = targetType.Id;
        bindingsTypeRegistrar.Register(targetTypeId, returnTypeId, [newBinding], true);
        resolvedBindingTypeRegistrar.Register(targetTypeId, returnTypeId, newResolvedBinding, true);
        return newResolvedBinding;
    }

    public ResolvedBinding TryCreateGenericSingleParameter(Type interfaceType, ClosedGenericType targetType, GenericBindingRegistration genericBindingRegistration)
    {
        var methodResult = methodFactory.CreateMethod(targetType, genericBindingRegistration.TargetType, genericBindingRegistration.Method);
        if (!methodResult.IsSuccess)
        {
            return ResolvedBinding.CreateGenericMethodError(methodResult.Error);
        }

        var newBinding = new Binding(
            targetType,
            targetType,
            genericBindingRegistration.Scope,
            methodResult.Value,
            genericBindingRegistration.HasLifecycle,
            false,
            genericBindingRegistration.IsNewOverridable);
        var resolvedBinding = ResolvedBinding.SingleParameter(newBinding);
        resolvedBindingTypeRegistrar.Register(targetType.Id, interfaceType.Id, resolvedBinding, true);
        return resolvedBinding;
    }

    public ResolvedBinding TryCreateMultiItemParameter(Type requestedType, Type elementType, ValueArray<BindingRegistration> resolvedBindingRegistrations, bool isArrayRequired)
    {
        var bindings = resolvedBindingRegistrations.Select(x =>
            new Binding(x.TargetType.Type, elementType, x.Scope, x.Method, x.TargetType.TypeMetadata.HasLifetime, x.IsInjectable, x.IsNewOverridable)).ToArray();

        bindingsTypeRegistrar.Register(requestedType.Id, default, bindings, true);
        return this.CreateMultiItemParameter(
            requestedType,
            elementType,
            bindings,
            isArrayRequired);
    }

    public ResolvedBinding CreateMultiItemParameter(
        Type requestedParameterType,
        Type elementType,
        IReadOnlyList<Binding> bindings,
        bool isArrayRequired)
    {
        var referencingType = isArrayRequired ? new ArrayType(elementType) : requestedParameterType;
        var multiItemParameter = ResolvedBinding.MultiItemParameter(referencingType, elementType, bindings, isArrayRequired);
        resolvedBindingTypeRegistrar.Register(requestedParameterType.Id, default, multiItemParameter, isArrayRequired);
        if (isArrayRequired)
        {
            var itemTypeArguments = ImmutableArray.Create(new TypeArgument(elementType, new TypeMetadata(default, EnumerableMetadata.NonEnumerableMetadata, false)));
            var iEnumerableOfItemType = knownEnumerableTypes.IEnumerableOfT.ToClosedGenericType(itemTypeArguments);
            var iReadOnlyListOfItemType = knownEnumerableTypes.IReadOnlyListOfT.ToClosedGenericType(itemTypeArguments);
            resolvedBindingTypeRegistrar.Register(iEnumerableOfItemType.Id, default, multiItemParameter, true);
            resolvedBindingTypeRegistrar.Register(iReadOnlyListOfItemType.Id, default, multiItemParameter, true);
        }

        return multiItemParameter;
    }

    public ResolvedBinding CreateFactoryBinding(
        NamedType factoryType,
        NamedType? factoryInterfaceType,
        ImmutableList<FactoryConstructorParameter>.Builder factoryConstructorParameters,
        bool hasLifecycle)
    {
        factoryInterfaceType ??= factoryType;
        var constructorMethod = new Method(
            factoryType,
            factoryType.Name,
            factoryConstructorParameters.Distinct()
                .Select(x => new Parameter(x.Type, x.Name, x.TypeMetadata, ParameterNecessity._Required))
                .ToImmutableArray(),
            ImmutableArray<TypeArgument>.Empty,
            MethodKind._Constructor);
        var binding = new Binding(factoryType, factoryInterfaceType, new ScopeContext(Scope._SingleInstancePerRequest(Location.None), ScopeSelection.Implicit), constructorMethod, hasLifecycle, false, false);
        var factoryInterfaceTypeId = factoryInterfaceType?.Id;
        var factoryTypeId = factoryType.Id;
        bindingsTypeRegistrar.Register(factoryTypeId, factoryInterfaceTypeId, [binding], true);
        var resolvedBinding = ResolvedBinding.SingleParameter(binding);
        resolvedBindingTypeRegistrar.Register(factoryTypeId, factoryInterfaceTypeId, resolvedBinding, true);
        nameTypeRegistrar.Register(factoryType.Name, factoryType);
        if (factoryInterfaceType.HasValue())
        {
            nameTypeRegistrar.Register(factoryInterfaceType.Name, factoryInterfaceType);
        }

        return resolvedBinding;
    }
}