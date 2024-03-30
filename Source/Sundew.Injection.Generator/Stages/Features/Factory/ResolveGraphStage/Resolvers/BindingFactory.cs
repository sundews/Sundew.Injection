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
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal class BindingFactory(
    TypeResolver typeResolver,
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
        var resolveTypeResult = typeResolver.ResolveType(bindingRegistration.TargetType.Type);
        if (!resolveTypeResult.IsSuccess)
        {
            return ResolvedBinding._Error(
                new BindingError.FailedResolveError(resolveTypeResult.Error));
        }

        var createMethodResult = methodFactory.CreateMethod(bindingRegistration.Method);
        if (!createMethodResult.IsSuccess)
        {
            return ResolvedBinding._Error(createMethodResult.Error);
        }

        var referencedTypeResult = typeResolver.ResolveType(bindingRegistration.ReferencedType);
        if (!referencedTypeResult.IsSuccess)
        {
            return ResolvedBinding._Error(new BindingError.FailedResolveError(referencedTypeResult.Error));
        }

        var resolvedTargetType = resolveTypeResult.Value;
        var newBinding = new Binding(resolvedTargetType, referencedTypeResult.Value, bindingRegistration.Scope, createMethodResult.Value, bindingRegistration.TargetType.TypeMetadata.HasLifetime, bindingRegistration.IsInjectable, bindingRegistration.IsNewOverridable);
        var newResolvedBinding = ResolvedBinding.SingleParameter(newBinding);
        var returnTypeId = returnType?.Id;
        var resolvedTargetTypeId = resolvedTargetType.Id;
        bindingsTypeRegistrar.Register(resolvedTargetTypeId, returnTypeId, [newBinding], true);
        resolvedBindingTypeRegistrar.Register(resolvedTargetTypeId, returnTypeId, newResolvedBinding, true);
        return newResolvedBinding;
    }

    public ResolvedBinding TryCreateGenericSingleParameter(Type interfaceType, DefiniteClosedGenericType targetType, GenericBindingRegistration genericBindingRegistration)
    {
        var methodResult = methodFactory.CreateMethod(targetType, genericBindingRegistration.TargetType, genericBindingRegistration.Method);
        if (!methodResult.IsSuccess)
        {
            return ResolvedBinding._Error(methodResult.Error);
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

    public ResolvedBinding TryCreateMultiItemParameter(DefiniteType requestedType, DefiniteType elementType, ValueArray<BindingRegistration> resolvedBindingRegistrations, bool isArrayRequired)
    {
        var createBindingsResult = resolvedBindingRegistrations.AllOrFailed(x =>
        {
            var resolveTypResult = typeResolver.ResolveType(x.TargetType.Type);
            if (!resolveTypResult.IsSuccess)
            {
                return Item.Fail<Binding, BindingError>(new BindingError.FailedResolveError(resolveTypResult.Error));
            }

            var createMethodResult = methodFactory.CreateMethod(x.Method);
            if (createMethodResult.IsSuccess)
            {
                var newBinding = new Binding(resolveTypResult.Value, elementType, x.Scope, createMethodResult.Value, x.TargetType.TypeMetadata.HasLifetime, x.IsInjectable, x.IsNewOverridable);
                return Item.Pass(newBinding);
            }

            return Item.Fail<Binding, BindingError>(createMethodResult.Error);
        });

        if (createBindingsResult.TryGet(out var all, out var failed))
        {
            bindingsTypeRegistrar.Register(requestedType.Id, default, all.Items, true);
            return this.CreateMultiItemParameter(
                requestedType,
                elementType,
                all.Items,
                isArrayRequired);
        }

        return ResolvedBinding._Error(BindingError.ResolveArrayElementsError(failed.Items.Select(x => x.Error).ToArray()));
    }

    public ResolvedBinding CreateMultiItemParameter(
        DefiniteType requestedParameterType,
        DefiniteType definiteElementType,
        IReadOnlyList<Binding> bindings,
        bool isArrayRequired)
    {
        var referencingType = isArrayRequired ? new DefiniteArrayType(definiteElementType) : requestedParameterType;
        var multiItemParameter = ResolvedBinding.MultiItemParameter(referencingType, definiteElementType, bindings, isArrayRequired);
        resolvedBindingTypeRegistrar.Register(requestedParameterType.Id, default, multiItemParameter, isArrayRequired);
        if (isArrayRequired)
        {
            var definiteItemTypeArguments = ImmutableArray.Create(new DefiniteTypeArgument(definiteElementType, new TypeMetadata(default, EnumerableMetadata.NonEnumerableMetadata, false)));
            var iEnumerableOfItemType = knownEnumerableTypes.IEnumerableOfT.ToDefiniteClosedGenericType(definiteItemTypeArguments);
            var iReadOnlyListOfItemType = knownEnumerableTypes.IReadOnlyListOfT.ToDefiniteClosedGenericType(definiteItemTypeArguments);
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
        var constructorMethod = new DefiniteMethod(
            factoryType,
            factoryType.Name,
            factoryConstructorParameters.Distinct()
                .Select(x => new DefiniteParameter(x.Type, x.Name, x.TypeMetadata, ParameterNecessity._Required))
                .ToImmutableArray(),
            ImmutableArray<DefiniteTypeArgument>.Empty,
            MethodKind._Constructor);
        var binding = new Binding(factoryType, factoryInterfaceType, Scope._SingleInstancePerRequest, constructorMethod, hasLifecycle, false, false);
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