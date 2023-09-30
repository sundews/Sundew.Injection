// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingFactory.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal class BindingFactory
{
    private readonly TypeResolver typeResolver;
    private readonly MethodFactory methodFactory;
    private readonly NameRegistry<NamedType> nameTypeRegistrar;
    private readonly ITypeRegistrar<ResolvedBinding> resolvedBindingTypeRegistrar;
    private readonly ITypeRegistrar<Binding[]> bindingsTypeRegistrar;

    public BindingFactory(
        TypeResolver typeResolver,
        MethodFactory methodFactory,
        NameRegistry<NamedType> nameTypeRegistrar,
        ITypeRegistrar<ResolvedBinding> resolvedBindingTypeRegistrar,
        ITypeRegistrar<Binding[]> bindingsTypeRegistrar)
    {
        this.typeResolver = typeResolver;
        this.methodFactory = methodFactory;
        this.nameTypeRegistrar = nameTypeRegistrar;
        this.resolvedBindingTypeRegistrar = resolvedBindingTypeRegistrar;
        this.bindingsTypeRegistrar = bindingsTypeRegistrar;
    }

    public ResolvedBinding TryCreateSingleParameter(BindingRegistration bindingRegistration, Type? returnType = null)
    {
        var resolveTypeResult = this.typeResolver.ResolveType(bindingRegistration.TargetType.Type);
        if (!resolveTypeResult.IsSuccess)
        {
            return ResolvedBinding._Error(
                new BindingError.FailedResolveError(resolveTypeResult.Error));
        }

        var createMethodResult = this.methodFactory.CreateMethod(bindingRegistration.Method);
        if (!createMethodResult.IsSuccess)
        {
            return ResolvedBinding._Error(createMethodResult.Error);
        }

        var commonTypeResult = this.typeResolver.ResolveType(bindingRegistration.TargetReferencingType);
        if (!commonTypeResult.IsSuccess)
        {
            return ResolvedBinding._Error(new BindingError.FailedResolveError(commonTypeResult.Error));
        }

        var resolvedTargetType = resolveTypeResult.Value;
        var newBinding = new Binding(resolvedTargetType, commonTypeResult.Value, bindingRegistration.Scope, createMethodResult.Value, bindingRegistration.TargetType.TypeMetadata.HasLifetime, bindingRegistration.IsInjectable, bindingRegistration.IsNewOverridable);
        var newResolvedBinding = ResolvedBinding.SingleParameter(newBinding);
        var returnTypeId = returnType?.Id;
        var resolvedTargetTypeId = resolvedTargetType.Id;
        this.bindingsTypeRegistrar.Register(resolvedTargetTypeId, returnTypeId, new[] { newBinding });
        this.resolvedBindingTypeRegistrar.Register(resolvedTargetTypeId, returnTypeId, newResolvedBinding);
        return newResolvedBinding;
    }

    public ResolvedBinding TryCreateGenericSingleParameter(Type interfaceType, DefiniteClosedGenericType targetType, GenericBindingRegistration genericBindingRegistration)
    {
        var methodResult = this.methodFactory.CreateMethod(targetType, genericBindingRegistration.TargetType, genericBindingRegistration.Method);
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
        this.resolvedBindingTypeRegistrar.Register(targetType.Id, interfaceType.Id, resolvedBinding);
        return resolvedBinding;
    }

    public ResolvedBinding TryCreateMultiItemParameter(Type requestedArrayCompatibleType, DefiniteType elementType, ValueArray<BindingRegistration> resolvedBindingRegistrationsForArray)
    {
        var createBindingsResult = resolvedBindingRegistrationsForArray.AllOrFailed(x =>
        {
            var resolveTypResult = this.typeResolver.ResolveType(x.TargetType.Type);
            if (!resolveTypResult.IsSuccess)
            {
                return Item.Fail<Binding, BindingError>(new BindingError.FailedResolveError(resolveTypResult.Error));
            }

            var createMethodResult = this.methodFactory.CreateMethod(x.Method);
            if (createMethodResult.IsSuccess)
            {
                var newBinding = new Binding(resolveTypResult.Value, elementType, x.Scope, createMethodResult.Value, x.TargetType.TypeMetadata.HasLifetime, x.IsInjectable, x.IsNewOverridable);
                return Item.Pass(newBinding);
            }

            return Item.Fail<Binding, BindingError>(createMethodResult.Error);
        });

        if (createBindingsResult.TryGet(out var all, out var failed))
        {
            this.bindingsTypeRegistrar.Register(requestedArrayCompatibleType.Id, null, all.Items);
            return this.CreateMultiItemParameter(requestedArrayCompatibleType, elementType, all.Items);
        }

        return ResolvedBinding._Error(new BindingError.ResolveArrayElementsError(failed.Items.Select(x => x.Error).ToArray()));
    }

    public ResolvedBinding CreateMultiItemParameter(Type requestedArrayCompatibleType, DefiniteType definiteElementType, IReadOnlyList<Binding> bindings)
    {
        var definiteArrayType = new DefiniteArrayType(definiteElementType);
        var arrayParameter = ResolvedBinding.MultiItemParameter(definiteArrayType, bindings);
        this.resolvedBindingTypeRegistrar.Register(definiteArrayType.Id, requestedArrayCompatibleType.Id, arrayParameter);
        return arrayParameter;
    }

    public ResolvedBinding CreateFactoryBinding(
        NamedType factoryType,
        NamedType? factoryInterfaceType,
        ImmutableList<FactoryConstructorParameter>.Builder factoryConstructorParameters,
        bool hasLifecycle)
    {
        var constructorMethod = new DefiniteMethod(factoryType, factoryType.Name, factoryConstructorParameters.Distinct().Select(x => new DefiniteParameter(x.Type, x.Name, x.TypeMetadata, ParameterNecessity._Required)).ToImmutableArray(), ImmutableArray<DefiniteTypeArgument>.Empty, MethodKind._Constructor);
        var binding = new Binding(factoryType, factoryType, Scope._Auto, constructorMethod, hasLifecycle, false, false);
        var bindings = new[] { binding };
        var factoryInterfaceTypeId = factoryInterfaceType?.Id;
        var factoryTypeId = factoryType.Id;
        this.bindingsTypeRegistrar.Register(factoryTypeId, factoryInterfaceTypeId, bindings);
        var resolvedBinding = ResolvedBinding.SingleParameter(binding);
        this.resolvedBindingTypeRegistrar.Register(factoryTypeId, factoryInterfaceTypeId, resolvedBinding);
        this.nameTypeRegistrar.Register(factoryType.Name, factoryType);
        if (factoryInterfaceType != null)
        {
            this.nameTypeRegistrar.Register(factoryInterfaceType.Name, factoryInterfaceType);
        }

        return resolvedBinding;
    }
}