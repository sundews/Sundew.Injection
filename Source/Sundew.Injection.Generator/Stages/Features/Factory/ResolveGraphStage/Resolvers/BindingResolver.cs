// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

using System.Collections.Immutable;
using System.Linq;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.Features;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using static ResolvedParameterSource;

internal sealed class BindingResolver
{
    private readonly BindingRegistrationResolver bindingRegistrationResolver;
    private readonly ValueDictionary<UnboundGenericType, ValueArray<GenericBindingRegistration>> genericBindingRegistrations;
    private readonly BindingFactory bindingFactory;
    private readonly ICache<TypeId, ResolvedBinding> resolvedBindingsCache;
    private readonly ICache<TypeId, Binding[]> bindingsCache;

    internal BindingResolver(
        BindingRegistrationResolver bindingRegistrationResolver,
        ValueDictionary<UnboundGenericType, ValueArray<GenericBindingRegistration>> genericBindingRegistrations,
        ImmutableArray<Binding> predefinedBindings,
        KnownEnumerableTypes knownEnumerableTypes)
    {
        this.bindingRegistrationResolver = bindingRegistrationResolver;
        this.genericBindingRegistrations = genericBindingRegistrations;
        var typeRegistry = new NameRegistry<NamedType>();
        var resolvedBindingRegistry = new TypeRegistry<ResolvedBinding>();
        var bindingsRegistry = new TypeRegistry<Binding[]>();
        foreach (var predefinedBinding in predefinedBindings)
        {
            bindingsRegistry.Register(predefinedBinding.TargetType.Id, predefinedBinding.ReferencedType.Id, [predefinedBinding], true);
        }

        this.bindingFactory = new BindingFactory(new MethodFactory(), typeRegistry, resolvedBindingRegistry, bindingsRegistry, knownEnumerableTypes);
        this.resolvedBindingsCache = resolvedBindingRegistry;
        this.bindingsCache = bindingsRegistry;
    }

    public void RegisterThisFactory(NamedType factoryType, NamedType? factoryInterfaceType)
    {
        this.bindingFactory.RegisterThisFactory(factoryType, factoryInterfaceType);
    }

    public ResolvedBinding ResolveBinding(FullParameter fullParameter, Type? dependantTypeOption, ParametersInjectionResolver parametersInjectionResolver)
    {
        if (this.resolvedBindingsCache.TryGet(fullParameter.Type.Id, out var cachedBinding))
        {
            return cachedBinding;
        }

        return this.ResolveParameter(fullParameter.Type, fullParameter.TypeMetadata, new RequestedParameterMetadata(fullParameter.Name, fullParameter.ParameterNecessity), dependantTypeOption, parametersInjectionResolver);
    }

    public ResolvedBinding ResolveBinding(
        Type type,
        TypeMetadata typeMetadata,
        Method? defaultConstructorOption,
        RequestedParameterMetadata? requestedParameterMetadataOption,
        Type? dependantTypeOption,
        ParametersInjectionResolver parametersInjectionResolver)
    {
        var typeId = type.Id;
        if (this.resolvedBindingsCache.TryGet(typeId, out var cachedBinding))
        {
            return cachedBinding;
        }

        if (this.bindingRegistrationResolver.TryGetValue(typeId, out var foundBindingRegistrations))
        {
            var bindingRegistration = foundBindingRegistrations.First();
            return this.bindingFactory.TryCreateSingleParameter(bindingRegistration, type);
        }

        if (requestedParameterMetadataOption.HasValue)
        {
            var resolvedParameterSource2 = parametersInjectionResolver.ResolveParameterSource(type, requestedParameterMetadataOption.Value.Name, dependantTypeOption);
            if (resolvedParameterSource2 is Found found2)
            {
                return ResolvedBinding.RequiredParameter(type, typeMetadata, found2.ParameterSource);
            }
        }

        if (defaultConstructorOption.TryGetValue(out var defaultConstructor))
        {
            return this.bindingFactory.TryCreateSingleParameter(new BindingRegistration(new FullType(type, typeMetadata, defaultConstructor), type, new ScopeContext(Scope._Auto, ScopeSelection.Implicit), defaultConstructor, false, false));
        }

        if (type is ClosedGenericType closedGenericType2)
        {
            var unboundGenericType = closedGenericType2.ToUnboundGenericType();
            if (this.genericBindingRegistrations.TryGetValue(unboundGenericType, out var resolvedGenericBindings))
            {
                var genericTypeDefinitionBinding = resolvedGenericBindings.First();
                var selectedUnboundGenericType = genericTypeDefinitionBinding.TargetType;
                var closedGenericType = selectedUnboundGenericType.ToClosedGenericType(closedGenericType2.TypeArguments);
                if (!this.resolvedBindingsCache.TryGet(closedGenericType.Id, out var resolvedBinding))
                {
                    return this.bindingFactory.TryCreateGenericSingleParameter(type, closedGenericType, genericTypeDefinitionBinding);
                }

                return resolvedBinding;
            }
        }

        if (typeMetadata.EnumerableMetadata.ImplementsIEnumerable)
        {
            if (type is ArrayType arrayType)
            {
                var resolvedBinding = this.ResolveMultiItemBinding(arrayType, arrayType.ElementType, true);
                if (resolvedBinding != null)
                {
                    return resolvedBinding;
                }
            }
            else if (type is ClosedGenericType closedGenericEnumerableType
                     && typeMetadata.EnumerableMetadata.IsArrayCompatible
                     && closedGenericEnumerableType.TypeArguments.TryGetOnlyOneValue(out var definiteItemTypeArgument))
            {
                var resolvedBinding = this.ResolveMultiItemBinding(closedGenericEnumerableType, definiteItemTypeArgument.Type, typeMetadata.EnumerableMetadata.IsArrayRequired);
                if (resolvedBinding != null)
                {
                    return resolvedBinding;
                }
            }
        }

        return this.ResolveParameter(type, typeMetadata, requestedParameterMetadataOption, dependantTypeOption, parametersInjectionResolver);
    }

    public BindingRoot CreateBindingRoot(FactoryMethodRegistration factoryMethodRegistration)
    {
        var targetType = factoryMethodRegistration.Target.Type;
        var returnType = factoryMethodRegistration.Return.Type;
        if (targetType == returnType)
        {
            if (this.bindingRegistrationResolver.TryGetValue(targetType.Id, out var registrations) && registrations.TryGetOnlyOne(out var registration))
            {
                returnType = registration.ReferencedType;
            }
        }

        var binding = new Binding(
            targetType,
            returnType,
            factoryMethodRegistration.Scope,
            factoryMethodRegistration.Method,
            factoryMethodRegistration.Target.Metadata.Lifecycle,
            factoryMethodRegistration.IsNewOverridable);
        return new BindingRoot(binding, factoryMethodRegistration.Accessibility, returnType);
    }

    public (NamedType FactoryType, NamedType? InterfaceType) CreateFactoryBinding(
        FactoryImplementationDefinition factoryImplementationDefinition,
        ValueArray<FullParameter> factoryConstructorParameters,
        Lifecycle lifecycle)
    {
        this.bindingFactory.CreateFactoryBinding(factoryImplementationDefinition.FactoryType, factoryImplementationDefinition.FactoryInterfaceType, factoryConstructorParameters, lifecycle);
        return (factoryImplementationDefinition.FactoryType, factoryImplementationDefinition.FactoryInterfaceType);
    }

    private ResolvedBinding? ResolveMultiItemBinding(
        Type parameterType,
        Type itemType,
        bool isArrayRequired)
    {
        var firstTypeArgumentTypeId = itemType.Id;
        if (this.bindingsCache.TryGet(firstTypeArgumentTypeId, out var bindings))
        {
            return this.bindingFactory.CreateMultiItemParameter(parameterType, itemType, bindings, isArrayRequired);
        }

        if (this.bindingRegistrationResolver.TryGetValue(firstTypeArgumentTypeId, out var resolvedBindingRegistrations))
        {
            return this.bindingFactory.TryCreateMultiItemParameter(parameterType, itemType, resolvedBindingRegistrations, isArrayRequired);
        }

        return default;
    }

    private ResolvedBinding ResolveParameter(Type type, TypeMetadata typeMetadata, RequestedParameterMetadata? requestedParameterMetadataOption, Type? dependantTypeOption, ParametersInjectionResolver parametersInjectionResolver)
    {
        if (requestedParameterMetadataOption is not { } requestedParameterMetadata)
        {
            return ResolvedBinding.RequiredParameter(type, typeMetadata, ParameterSource.DirectParameter(type, type.Name.Uncapitalize(), false, false, ParameterNecessity._Required, Inject.Shared));
        }

        return requestedParameterMetadata.ParameterNecessity switch
        {
            ParameterNecessity.Optional optional => EvaluateParameter(type, typeMetadata, optional),
            ParameterNecessity.Required => EvaluateParameter(type, typeMetadata, null),
        };

        ResolvedBinding EvaluateParameter(Type type, TypeMetadata typeMetadata, ParameterNecessity.Optional? optionalOption)
        {
            var parameterName = requestedParameterMetadata.Name;
            var resolvedParameterSource = parametersInjectionResolver.ResolveParameterSource(type, parameterName, dependantTypeOption);
            return resolvedParameterSource switch
            {
                Found found => ResolvedBinding.RequiredParameter(type, typeMetadata, found.ParameterSource),
                NotFound notFound => optionalOption.HasValue ? ResolvedBinding.OptionalParameter(optionalOption.DefaultValue, type, typeMetadata) : ResolvedBinding.RequiredParameter(type, typeMetadata, notFound.ProposedParameterSource),
                NoExactMatch noExactMatch => optionalOption.HasValue ? ResolvedBinding.OptionalParameter(optionalOption.DefaultValue, type, typeMetadata) : ResolvedBindingError.ParameterError(type, parameterName, noExactMatch.ParameterSources),
            };
        }
    }
}