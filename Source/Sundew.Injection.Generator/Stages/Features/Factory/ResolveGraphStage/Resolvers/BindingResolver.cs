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
    private const string Create = "Create";
    private readonly ValueDictionary<TypeId, ValueArray<BindingRegistration>> bindingRegistrations;
    private readonly ValueDictionary<UnboundGenericType, ValueArray<GenericBindingRegistration>> genericBindingRegistrations;
    private readonly BindingFactory bindingFactory;
    private readonly ICache<TypeId, ResolvedBinding> resolvedBindingsCache;
    private readonly ICache<TypeId, Binding[]> bindingsCache;
    private readonly RequiredParametersInjectionResolver requiredParametersInjectionResolver;

    internal BindingResolver(
        ValueDictionary<TypeId, ValueArray<BindingRegistration>> bindingRegistrations,
        ValueDictionary<UnboundGenericType, ValueArray<GenericBindingRegistration>> genericBindingRegistrations,
        RequiredParametersInjectionResolver requiredParametersInjectionResolver,
        ImmutableArray<Binding> predefinedBindings,
        KnownEnumerableTypes knownEnumerableTypes)
    {
        this.bindingRegistrations = bindingRegistrations;
        this.genericBindingRegistrations = genericBindingRegistrations;
        this.requiredParametersInjectionResolver = requiredParametersInjectionResolver;
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

    public ResolvedBinding ResolveBinding(FullParameter fullParameter)
    {
        if (this.resolvedBindingsCache.TryGet(fullParameter.Type.Id, out var cachedBinding))
        {
            return cachedBinding;
        }

        return this.ResolveParameter(fullParameter.Type, fullParameter.TypeMetadata, (fullParameter.Name, fullParameter.ParameterNecessity));
    }

    public ResolvedBinding ResolveBinding(Type type, TypeMetadata typeMetadata, Method? defaultConstructorOption, (string Name, ParameterNecessity Necessity)? parameterOption)
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

        if (parameterOption.HasValue)
        {
            var resolvedParameterSource = this.requiredParametersInjectionResolver.ResolveParameterSource(type, parameterOption.Value.Name);
            if (resolvedParameterSource is Found found)
            {
                return ResolvedBinding.RequiredParameter(type, typeMetadata, found.ParameterSource);
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
                     && closedGenericEnumerableType.TypeArguments.TryGetOnlyOne(out var definiteItemTypeArgument))
            {
                var resolvedBinding = this.ResolveMultiItemBinding(closedGenericEnumerableType, definiteItemTypeArgument.Type, typeMetadata.EnumerableMetadata.IsArrayRequired);
                if (resolvedBinding != null)
                {
                    return resolvedBinding;
                }
            }
        }

        return this.ResolveParameter(type, typeMetadata, parameterOption);
    }

    public BindingRoot CreateBindingRoot(FactoryMethodRegistration factoryMethodRegistration, bool useTargetTypeNameForCreateMethod)
    {
        string GetName()
        {
            return useTargetTypeNameForCreateMethod ? factoryMethodRegistration.Target.Type.Name : string.Empty;
        }

        var factoryMethodName = factoryMethodRegistration.CreateMethodName.IsNullOrEmpty()
            ? factoryMethodRegistration.Scope.Scope is Scope.SingleInstancePerFactory
                ? GetName()
                : Create + GetName()
            : factoryMethodRegistration.CreateMethodName;
        var binding = new Binding(
            factoryMethodRegistration.Target.Type,
            factoryMethodRegistration.Return.Type,
            factoryMethodRegistration.Scope,
            factoryMethodRegistration.Method with { Name = factoryMethodName },
            factoryMethodRegistration.Target.Metadata.HasLifecycle,
            false,
            factoryMethodRegistration.IsNewOverridable);
        return new BindingRoot(binding, factoryMethodRegistration.Accessibility, factoryMethodRegistration.Return.Type);
    }

    public (NamedType FactoryType, NamedType? InterfaceType) CreateFactoryBinding(
        FactoryCreationDefinition factoryCreationDefinition,
        ImmutableList<FactoryConstructorParameter>.Builder factoryConstructorParameters,
        bool needLifecycleHandling)
    {
        this.bindingFactory.CreateFactoryBinding(factoryCreationDefinition.FactoryType, factoryCreationDefinition.FactoryInterfaceType, factoryConstructorParameters, needLifecycleHandling);
        return (factoryCreationDefinition.FactoryType, factoryCreationDefinition.FactoryInterfaceType);
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

        if (this.bindingRegistrations.TryGetValue(firstTypeArgumentTypeId, out var resolvedBindingRegistrations))
        {
            return this.bindingFactory.TryCreateMultiItemParameter(parameterType, itemType, resolvedBindingRegistrations, isArrayRequired);
        }

        return default;
    }

    private ResolvedBinding ResolveParameter(Type type, TypeMetadata typeMetadata, (string Name, ParameterNecessity Necessity)? parameterOption)
    {
        if (!parameterOption.HasValue)
        {
            return ResolvedBinding.RequiredParameter(type, typeMetadata, ParameterSource.DirectParameter(this.requiredParametersInjectionResolver.Inject));
        }

        return parameterOption.Value.Necessity switch
        {
            ParameterNecessity.Optional optional => EvaluateParameter(type, typeMetadata, optional),
            ParameterNecessity.Required => EvaluateParameter(type, typeMetadata, null),
        };

        ResolvedBinding EvaluateParameter(Type type, TypeMetadata typeMetadata, ParameterNecessity.Optional? optionalOption)
        {
            var parameterName = parameterOption.Value.Name;
            var resolvedParameterSource = this.requiredParametersInjectionResolver.ResolveParameterSource(type, parameterName);
            return resolvedParameterSource switch
            {
                Found found => ResolvedBinding.RequiredParameter(type, typeMetadata, found.ParameterSource),
                NotFound notFound => optionalOption.HasValue() ? ResolvedBinding.OptionalParameter(optionalOption.DefaultValue, type, typeMetadata) : ResolvedBinding.RequiredParameter(type, typeMetadata, notFound.ProposedParameterSource),
                NoExactMatch noExactMatch => optionalOption.HasValue() ? ResolvedBinding.OptionalParameter(optionalOption.DefaultValue, type, typeMetadata) : ResolvedBindingError.ParameterError(type, parameterName, noExactMatch.ParameterSources),
            };
        }
    }
}