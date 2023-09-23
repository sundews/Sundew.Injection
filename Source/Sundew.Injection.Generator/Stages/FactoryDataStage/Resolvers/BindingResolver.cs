// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers;

using System;
using System.Collections.Immutable;
using System.Linq;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Primitives.Computation;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using static Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers.ResolvedParameterSource;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

internal sealed class BindingResolver
{
    private const string Create = "Create";
    private readonly TypeResolver typeResolver;
    private readonly ValueDictionary<TypeId, ValueArray<BindingRegistration>> bindingRegistrations;
    private readonly ValueDictionary<UnboundGenericType, ValueArray<GenericBindingRegistration>> genericBindingRegistrations;
    private readonly MethodFactory methodFactory;
    private readonly BindingFactory bindingFactory;
    private readonly ICache<TypeId, ResolvedBinding> resolvedBindingsCache;
    private readonly ICache<TypeId, Binding[]> bindingsCache;
    private readonly RequiredParametersInjectionResolver requiredParametersInjectionResolver;

    internal BindingResolver(
        ValueDictionary<TypeId, ValueArray<BindingRegistration>> bindingRegistrations,
        ValueDictionary<UnboundGenericType, ValueArray<GenericBindingRegistration>> genericBindingRegistrations,
        RequiredParametersInjectionResolver requiredParametersInjectionResolver,
        ImmutableArray<Binding> predefinedBindings)
    {
        this.bindingRegistrations = bindingRegistrations;
        this.genericBindingRegistrations = genericBindingRegistrations;
        this.requiredParametersInjectionResolver = requiredParametersInjectionResolver;
        var typeRegistry = new NameRegistry<NamedType>();
        this.typeResolver = new TypeResolver(typeRegistry);
        this.methodFactory = new MethodFactory(this.typeResolver);
        var resolvedBindingRegistry = new TypeRegistry<ResolvedBinding>();
        var bindingsRegistry = new TypeRegistry<Binding[]>();
        foreach (var predefinedBinding in predefinedBindings)
        {
            bindingsRegistry.Register(predefinedBinding.TargetType.Id, predefinedBinding.TargetReferenceType.Id, new[] { predefinedBinding });
        }

        this.bindingFactory = new BindingFactory(this.typeResolver, this.methodFactory, typeRegistry, resolvedBindingRegistry, bindingsRegistry);
        this.resolvedBindingsCache = resolvedBindingRegistry;
        this.bindingsCache = bindingsRegistry;
    }

    public ResolvedBinding ResolveBinding(DefiniteParameter definiteParameter)
    {
        if (this.resolvedBindingsCache.TryGet(definiteParameter.Type.Id, out var cachedBinding))
        {
            return cachedBinding;
        }

        return this.ResolveParameter(definiteParameter.Type, definiteParameter.TypeMetadata, O.Some((definiteParameter.Name, definiteParameter.ParameterNecessity)));
    }

    public ResolvedBinding ResolveBinding(Type type, TypeMetadata typeMetadata, O<(string Name, ParameterNecessity Necessity)> parameterOption)
    {
        var typeId = type.Id;
        if (this.resolvedBindingsCache.TryGet(typeId, out var cachedBinding))
        {
            return cachedBinding;
        }

        if (this.bindingRegistrations.TryGetValue(typeId, out var foundBindingRegistrations))
        {
            var bindingRegistration = foundBindingRegistrations.First();
            return this.bindingFactory.TryCreateSingleParameter(bindingRegistration, type);
        }

        if (typeMetadata.DefaultConstructor.HasValue)
        {
            return this.bindingFactory.TryCreateSingleParameter(new BindingRegistration((type, typeMetadata), type, Scope._Auto, typeMetadata.DefaultConstructor.Value, false, false));
        }

        if (type is DefiniteBoundGenericType definiteBoundGenericType2)
        {
            var unboundGenericType = definiteBoundGenericType2.ToUnboundGenericType();
            if (this.genericBindingRegistrations.TryGetValue(unboundGenericType, out var resolvedGenericBindings))
            {
                var genericTypeDefinitionBinding = resolvedGenericBindings.First();
                var selectedUnboundGenericType = genericTypeDefinitionBinding.TargetType;
                var definiteBoundGenericTargetType = selectedUnboundGenericType.ToDefiniteBoundGenericType(definiteBoundGenericType2.TypeArguments);
                if (!this.resolvedBindingsCache.TryGet(definiteBoundGenericTargetType.Id, out var resolvedBinding))
                {
                    return this.bindingFactory.TryCreateGenericSingleParameter(type, definiteBoundGenericTargetType, genericTypeDefinitionBinding);
                }

                return resolvedBinding;
            }
        }

        if (typeMetadata.ImplementsIEnumerable)
        {
            if (type is DefiniteArrayType definiteArrayType)
            {
                var resolvedBinding = this.ResolveArrayBinding(definiteArrayType.ElementType, type);
                if (resolvedBinding != null)
                {
                    return resolvedBinding;
                }
            }
            else if (type is ArrayType arrayType)
            {
                var resolvedBinding = this.ResolveArrayBinding(arrayType.ElementType, type);
                if (resolvedBinding != null)
                {
                    return resolvedBinding;
                }
            }
            else if (type is BoundGenericType definiteBoundGenericType)
            {
                var firstTypeArgumentType = definiteBoundGenericType.TypeArguments.First().Type;
                var resolvedBinding = this.ResolveArrayBinding(firstTypeArgumentType, type);
                if (resolvedBinding != null)
                {
                    return resolvedBinding;
                }
            }
            else if (type is DefiniteBoundGenericType definiteBoundGenericTypeEnumerable)
            {
                var firstTypeArgumentType = definiteBoundGenericTypeEnumerable.TypeArguments.First().Type;
                var resolvedBinding = this.ResolveArrayBinding(firstTypeArgumentType, type);
                if (resolvedBinding != null)
                {
                    return resolvedBinding;
                }
            }
        }

        var resolveTypeResultParameter = this.typeResolver.ResolveType(type);
        if (resolveTypeResultParameter.IsSuccess)
        {
            return this.ResolveParameter(resolveTypeResultParameter.Value, typeMetadata, parameterOption);
        }

        return ResolvedBinding._Error(new BindingError.FailedResolveError(ImmutableArray.Create(resolveTypeResultParameter.Error)));
    }

    public R<BindingRoot, BindingError> CreateBindingRoot(FactoryMethodRegistration factoryMethodRegistration, bool useTargetTypeNameForCreateMethod)
    {
        var returnTypeResult = this.typeResolver.ResolveType(factoryMethodRegistration.Return.Type);
        var targetTypeResult = this.typeResolver.ResolveType(factoryMethodRegistration.Target.Type);
        if (returnTypeResult.IsSuccess && targetTypeResult.IsSuccess)
        {
            var factoryMethodName = factoryMethodRegistration.CreateMethodName.IsNullOrEmpty()
                ? Create + (useTargetTypeNameForCreateMethod ? targetTypeResult.Value.Name : string.Empty)
                : factoryMethodRegistration.CreateMethodName;
            var createMethodResult = this.methodFactory.CreateMethod(factoryMethodRegistration.Method, factoryMethodName);
            if (createMethodResult.IsSuccess)
            {
                var binding = new Binding(targetTypeResult.Value, returnTypeResult.Value, factoryMethodRegistration.Scope, createMethodResult.Value, factoryMethodRegistration.Target.TypeMetadata.HasLifetime, false, factoryMethodRegistration.IsNewOverridable);
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
        NamedType fallbackFactoryType,
        ImmutableList<FactoryConstructorParameter>.Builder factoryConstructorParameters,
        bool needLifecycleHandling,
        string assemblyName)
    {
        var (classTypeName, interfaceTypeName, factoryNamespace) = FactoryNameHelper.GetFactoryNames(
            factoryCreationDefinition.FactoryClassNamespace,
            factoryCreationDefinition.FactoryClassName,
            fallbackFactoryType.Namespace,
            fallbackFactoryType.Name);

        var factoryType = new NamedType(
            classTypeName,
            factoryNamespace,
            assemblyName);

        NamedType? factoryInterfaceType = null;
        if (factoryCreationDefinition.GenerateInterface)
        {
            factoryInterfaceType = new NamedType(interfaceTypeName, factoryNamespace, assemblyName);
        }

        this.bindingFactory.CreateFactoryBinding(factoryType, factoryInterfaceType, factoryConstructorParameters, needLifecycleHandling);
        return (factoryType, factoryInterfaceType);
    }

    private ResolvedBinding? ResolveArrayBinding(Type firstTypeArgumentType, Type parameterType)
    {
        var elementTypeLookupResult = this.typeResolver.ResolveType(firstTypeArgumentType);
        if (!elementTypeLookupResult.IsSuccess)
        {
            return ResolvedBinding._Error(new BindingError.FailedResolveError(elementTypeLookupResult.Error));
        }

        var firstTypeArgumentTypeId = firstTypeArgumentType.Id;
        if (this.bindingsCache.TryGet(firstTypeArgumentTypeId, out var bindings))
        {
            return this.bindingFactory.CreateArrayParameter(parameterType, elementTypeLookupResult.Value, bindings);
        }

        if (this.bindingRegistrations.TryGetValue(firstTypeArgumentTypeId, out var resolvedBindingRegistrationsForArray))
        {
            return this.bindingFactory.TryCreateArrayParameter(parameterType, elementTypeLookupResult.Value, resolvedBindingRegistrationsForArray);
        }

        return null;
    }

    private ResolvedBinding ResolveParameter(DefiniteType definiteType, TypeMetadata typeMetadata, O<(string Name, ParameterNecessity Necessity)> parameterOption)
    {
        if (!parameterOption.HasValue)
        {
            return ResolvedBinding.ExternalParameter(definiteType, typeMetadata, ParameterSource.DirectParameter(this.requiredParametersInjectionResolver.Inject));
        }

        return parameterOption.Value.Necessity switch
        {
            ParameterNecessity.Optional optional => EvaluateParameter(definiteType, typeMetadata, O.Some(optional)),
            ParameterNecessity.Required => EvaluateParameter(definiteType, typeMetadata, O.None),
        };

        ResolvedBinding EvaluateParameter(DefiniteType definiteType, TypeMetadata typeMetadata, O<ParameterNecessity.Optional> optionalOption)
        {
            var parameterName = parameterOption.Value.Name;
            var resolvedParameterSource = this.requiredParametersInjectionResolver.ResolveParameterSource(definiteType, parameterName);
            return resolvedParameterSource switch
            {
                Found found => ResolvedBinding.ExternalParameter(definiteType, typeMetadata, found.ParameterSource),
                NotFound notFound => optionalOption.HasValue ? ResolvedBinding.DefaultParameter(optionalOption.Value.DefaultValue, definiteType, typeMetadata) : ResolvedBinding.ExternalParameter(definiteType, typeMetadata, notFound.ProposedParameterSource),
                NoExactMatch noExactMatch => optionalOption.HasValue ? ResolvedBinding.DefaultParameter(optionalOption.Value.DefaultValue, definiteType, typeMetadata) : ResolvedBinding._ParameterError(definiteType, parameterName, noExactMatch.ParameterSources),
            };
        }
    }
}