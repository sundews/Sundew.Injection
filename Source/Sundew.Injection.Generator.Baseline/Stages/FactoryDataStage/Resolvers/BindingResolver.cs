// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers;

using System.Collections.Immutable;
using System.Linq;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;
using Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

internal sealed class BindingResolver
{
    private readonly TypeResolver typeResolver;
    private readonly ValueDictionary<Type, ValueArray<BindingRegistration>> bindingRegistrations;
    private readonly ValueDictionary<UnboundGenericType, ValueArray<GenericBindingRegistration>> genericBindingRegistrations;
    private readonly MethodFactory methodFactory;
    private readonly BindingFactory bindingFactory;
    private readonly ICache<Type, ResolvedBinding> resolvedBindingsCache;
    private readonly ICache<Type, Binding[]> bindingsCache;

    internal BindingResolver(
        ValueDictionary<Type, ValueArray<BindingRegistration>> bindingRegistrations,
        ValueDictionary<UnboundGenericType, ValueArray<GenericBindingRegistration>> genericBindingRegistrations)
    {
        this.bindingRegistrations = bindingRegistrations;
        this.genericBindingRegistrations = genericBindingRegistrations;
        var typeRegistry = new NameRegistry<NamedType>();
        this.typeResolver = new TypeResolver(typeRegistry);
        this.methodFactory = new MethodFactory(this.typeResolver);
        var resolvedBindingRegistry = new TypeRegistry<ResolvedBinding>();
        var bindingsRegistry = new TypeRegistry<Binding[]>();
        this.bindingFactory = new BindingFactory(this.typeResolver, this.methodFactory, typeRegistry, resolvedBindingRegistry, bindingsRegistry);
        this.resolvedBindingsCache = resolvedBindingRegistry;
        this.bindingsCache = bindingsRegistry;
    }

    public ResolvedBinding ResolveBinding(DefiniteParameter definiteParameter)
    {
        if (this.resolvedBindingsCache.TryGet(definiteParameter.Type, out var cachedBinding))
        {
            return cachedBinding;
        }

        return ResolvedBinding.ExternalParameter(definiteParameter.Type, definiteParameter.TypeMetadata);
    }

    public ResolvedBinding ResolveBinding(Type type, TypeMetadata typeMetadata)
    {
        if (this.resolvedBindingsCache.TryGet(type, out var cachedBinding))
        {
            return cachedBinding;
        }

        if (this.bindingRegistrations.TryGetValue(type, out var foundBindingRegistrations))
        {
            var bindingRegistration = foundBindingRegistrations.First();
            return this.bindingFactory.TryCreateSingleParameter(bindingRegistration, type);
        }

        if (typeMetadata.DefaultConstructor != default)
        {
            return this.bindingFactory.TryCreateSingleParameter(new BindingRegistration(type, type, Scope.Auto, typeMetadata.DefaultConstructor, typeMetadata.ImplementsIDisposable, false, false));
        }

        if (typeMetadata.ImplementsIEnumerable)
        {
            if (type is DefiniteArrayType definiteArrayType)
            {
                var resolvedBinding = this.ResolveArrayBinding(definiteArrayType.ElementType, type);
                if (resolvedBinding != default)
                {
                    return resolvedBinding;
                }
            }
            else if (type is ArrayType arrayType)
            {
                var resolvedBinding = this.ResolveArrayBinding(arrayType.ElementType, type);
                if (resolvedBinding != default)
                {
                    return resolvedBinding;
                }
            }
            else if (type is BoundGenericType definiteBoundGenericType)
            {
                var firstTypeArgumentType = definiteBoundGenericType.TypeArguments.First().Type;
                var resolvedBinding = this.ResolveArrayBinding(firstTypeArgumentType, type);
                if (resolvedBinding != default)
                {
                    return resolvedBinding;
                }
            }
        }

        if (type is DefiniteBoundGenericType definiteBoundGenericType2)
        {
            var unboundGenericType = definiteBoundGenericType2.ToUnboundGenericType();
            if (this.genericBindingRegistrations.TryGetValue(unboundGenericType, out var resolvedGenericBindings))
            {
                var genericTypeDefinitionBinding = resolvedGenericBindings.First();
                var selectedUnboundGenericType = genericTypeDefinitionBinding.TargetType;
                var definiteBoundGenericTargetType = selectedUnboundGenericType.ToDefiniteBoundGenericType(definiteBoundGenericType2.TypeArguments);
                if (!this.resolvedBindingsCache.TryGet(definiteBoundGenericTargetType, out var resolvedBinding))
                {
                    return this.bindingFactory.TryCreateGenericSingleParameter(type, definiteBoundGenericTargetType, genericTypeDefinitionBinding);
                }

                return resolvedBinding;
            }
        }

        var resolveTypeResultForExternalParameter = this.typeResolver.ResolveType(type);
        if (resolveTypeResultForExternalParameter.IsSuccess)
        {
            return ResolvedBinding.ExternalParameter(resolveTypeResultForExternalParameter.Value, typeMetadata);
        }

        return ResolvedBinding.Error(new BindingError.FailedResolveError(ImmutableArray.Create(resolveTypeResultForExternalParameter.Error)));
    }

    public R<BindingRoot, BindingError> CreateBindingRoot(FactoryMethodRegistration factoryMethodRegistration, bool useTargetTypeNameForCreateMethod)
    {
        var returnTypeResult = this.typeResolver.ResolveType(factoryMethodRegistration.Return.Type);
        var targetTypeResult = this.typeResolver.ResolveType(factoryMethodRegistration.Target.Type);
        if (returnTypeResult.IsSuccess && targetTypeResult.IsSuccess)
        {
            var factoryMethodName = factoryMethodRegistration.CreateMethodName.IsNullOrEmpty()
                ? "Create" + (useTargetTypeNameForCreateMethod ? targetTypeResult.Value.Name : string.Empty)
                : factoryMethodRegistration.CreateMethodName;
            var createMethodResult = this.methodFactory.CreateMethod(factoryMethodRegistration.Method, factoryMethodName);
            if (createMethodResult.IsSuccess)
            {
                var binding = new Binding(targetTypeResult.Value, targetTypeResult.Value, Scope.Auto, createMethodResult.Value, factoryMethodRegistration.Target.TypeMetadata.ImplementsIDisposable, false, factoryMethodRegistration.IsNewOverridable);
                return R.Success(new BindingRoot(binding, factoryMethodRegistration.Accessibility, returnTypeResult.Value));
            }

            return R.Error<BindingError>(createMethodResult.Error);
        }

        var failedResolves = ImmutableArray<FailedResolve>.Empty;
        if (!returnTypeResult.IsSuccess)
        {
            failedResolves = failedResolves.Add(returnTypeResult.Error);
        }

        if (!targetTypeResult.IsSuccess)
        {
            failedResolves = failedResolves.Add(targetTypeResult.Error);
        }

        return R.Error<BindingError>(new BindingError.FailedResolveError(failedResolves));
    }

    public (NamedType FactoryType, NamedType? InterfaceType) CreateFactoryBinding(
        FactoryCreationDefinition factoryCreationDefinition,
        FactoryMethodData fallbackFactoryMethodData,
        ImmutableList<FactoryConstructorParameterInjectionNode>.Builder factoryConstructorParameters,
        bool implementsIDisposable,
        string assemblyName)
    {
        var (classTypeName, interfaceTypeName, factoryNamespace) = FactoryNameHelper.GetFactoryNames(
            factoryCreationDefinition.FactoryClassNamespace,
            factoryCreationDefinition.FactoryClassName,
            fallbackFactoryMethodData.Target.Type.Namespace,
            fallbackFactoryMethodData.Return.Type.Name);

        var factoryType = new NamedType(
            classTypeName,
            factoryNamespace,
            assemblyName);

        NamedType? factoryInterfaceType = default;
        if (factoryCreationDefinition.GenerateInterface)
        {
            factoryInterfaceType = new NamedType(interfaceTypeName, factoryNamespace, assemblyName);
        }

        this.bindingFactory.CreateFactoryBinding(factoryType, factoryInterfaceType, factoryConstructorParameters, implementsIDisposable);
        return (factoryType, factoryInterfaceType);
    }

    private ResolvedBinding? ResolveArrayBinding(Type firstTypeArgumentType, Type parameterType)
    {
        var elementTypeLookupResult = this.typeResolver.ResolveType(firstTypeArgumentType);
        if (!elementTypeLookupResult.IsSuccess)
        {
            return ResolvedBinding.Error(new BindingError.FailedResolveError(elementTypeLookupResult.Error));
        }

        if (this.bindingsCache.TryGet(firstTypeArgumentType, out var bindings))
        {
            return this.bindingFactory.CreateArrayParameter(parameterType, elementTypeLookupResult.Value, bindings);
        }

        if (this.bindingRegistrations.TryGetValue(firstTypeArgumentType, out var resolvedBindingRegistrationsForArray))
        {
            return this.bindingFactory.TryCreateArrayParameter(parameterType, elementTypeLookupResult.Value, resolvedBindingRegistrationsForArray);
        }

        return default;
    }
}