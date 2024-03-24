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
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;
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
        var resolveTypeResult = this.typeResolver.ResolveType(bindingRegistration.TargetType);
        if (!resolveTypeResult.IsSuccess)
        {
            return ResolvedBinding.Error(
                new BindingError.FailedResolveError(resolveTypeResult.Error));
        }

        var createMethodResult = this.methodFactory.CreateMethod(bindingRegistration.Method);
        if (!createMethodResult.IsSuccess)
        {
            return ResolvedBinding.Error(createMethodResult.Error);
        }

        var commonTypeResult = this.typeResolver.ResolveType(bindingRegistration.TargetReferencingType);
        if (!commonTypeResult.IsSuccess)
        {
            return ResolvedBinding.Error(new BindingError.FailedResolveError(commonTypeResult.Error));
        }

        var resolvedTargetType = resolveTypeResult.Value;
        var newBinding = new Binding(resolvedTargetType, commonTypeResult.Value, bindingRegistration.Scope, createMethodResult.Value, bindingRegistration.ImplementsIDisposable, bindingRegistration.IsInjectable, bindingRegistration.IsNewOverridable);
        var newResolvedBinding = ResolvedBinding.SingleParameter(newBinding);
        this.bindingsTypeRegistrar.Register(resolvedTargetType, returnType, new[] { newBinding });
        this.resolvedBindingTypeRegistrar.Register(resolvedTargetType, returnType, newResolvedBinding);
        return newResolvedBinding;
    }

    public ResolvedBinding TryCreateGenericSingleParameter(Type interfaceType, DefiniteBoundGenericType targetType, GenericBindingRegistration genericBindingRegistration)
    {
        var methodResult = this.methodFactory.CreateMethod(targetType, genericBindingRegistration.TargetType, genericBindingRegistration.Method);
        if (!methodResult.IsSuccess)
        {
            return ResolvedBinding.Error(methodResult.Error);
        }

        var newBinding = new Binding(
            targetType,
            targetType,
            genericBindingRegistration.Scope,
            methodResult.Value,
            genericBindingRegistration.ImplementsIDisposable,
            false,
            genericBindingRegistration.IsNewOverridable);
        var resolvedBinding = ResolvedBinding.SingleParameter(newBinding);
        this.resolvedBindingTypeRegistrar.Register(targetType, interfaceType, resolvedBinding);
        return resolvedBinding;
    }

    public ResolvedBinding TryCreateArrayParameter(Type requestedArrayCompatibleType, DefiniteType elementType, ValueArray<BindingRegistration> resolvedBindingRegistrationsForArray)
    {
        var createBindingsResult = resolvedBindingRegistrationsForArray.AllOrFailed(x =>
        {
            var resolveTypResult = this.typeResolver.ResolveType(x.TargetType);
            if (!resolveTypResult.IsSuccess)
            {
                return Item.Fail<Binding, BindingError>(new BindingError.FailedResolveError(resolveTypResult.Error));
            }

            var createMethodResult = this.methodFactory.CreateMethod(x.Method);
            if (createMethodResult.IsSuccess)
            {
                var newBinding = new Binding(resolveTypResult.Value, elementType, x.Scope, createMethodResult.Value, x.ImplementsIDisposable, x.IsInjectable, x.IsNewOverridable);
                return Item.Pass(newBinding);
            }

            return Item.Fail<Binding, BindingError>(createMethodResult.Error);
        });

        if (createBindingsResult.TryGet(out var all, out var failed))
        {
            this.bindingsTypeRegistrar.Register(requestedArrayCompatibleType, null, all.Items);
            return this.CreateArrayParameter(requestedArrayCompatibleType, elementType, all.Items);
        }

        return ResolvedBinding.Error(new BindingError.ResolveArrayElementsError(failed.Items.Select(x => x.Error).ToArray()));
    }

    public ResolvedBinding CreateArrayParameter(Type requestedArrayCompatibleType, DefiniteType definiteElementType, IReadOnlyList<Binding> bindings)
    {
        var definiteArrayType = new DefiniteArrayType(definiteElementType);
        var arrayParameter = ResolvedBinding.ArrayParameter(definiteArrayType, bindings);
        this.resolvedBindingTypeRegistrar.Register(definiteArrayType, requestedArrayCompatibleType, arrayParameter);
        return arrayParameter;
    }

    public ResolvedBinding CreateFactoryBinding(
        NamedType factoryType,
        NamedType? factoryInterfaceType,
        ImmutableList<FactoryConstructorParameterInjectionNode>.Builder factoryConstructorParameters,
        bool implementsIDisposable)
    {
        var constructorMethod = new DefiniteMethod(factoryConstructorParameters.Distinct().Select(x => new DefiniteParameter(x.Type, x.Name, x.TypeMetadata)).ToImmutableArray(), factoryType.Name, factoryType, ImmutableArray<DefiniteTypeArgument>.Empty, true);
        var binding = new Binding(factoryType, factoryType, Scope.Auto, constructorMethod, implementsIDisposable, false, false);
        var bindings = new[] { binding };
        this.bindingsTypeRegistrar.Register(factoryType, factoryInterfaceType, bindings);
        var resolvedBinding = ResolvedBinding.SingleParameter(binding);
        this.resolvedBindingTypeRegistrar.Register(factoryType, factoryInterfaceType, resolvedBinding);
        this.nameTypeRegistrar.Register(factoryType.Name, factoryType);
        if (factoryInterfaceType != null)
        {
            this.nameTypeRegistrar.Register(factoryInterfaceType.Name, factoryInterfaceType);
        }

        return resolvedBinding;
    }
}