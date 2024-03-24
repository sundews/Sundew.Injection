// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompiletimeInjectionDefinitionBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;
using Accessibility = Sundew.Injection.Accessibility;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

internal sealed class CompiletimeInjectionDefinitionBuilder : IInjectionDefinitionBuilder
{
    private readonly Dictionary<TypeId, List<BindingRegistration>> bindingRegistrations = new();

    private readonly Dictionary<UnboundGenericType, List<GenericBindingRegistration>> genericBindingRegistrations = new();

    private readonly Dictionary<TypeId, List<ParameterSource>> requiredParameters = new();

    private readonly List<FactoryCreationDefinition> factoryDefinitions = new();

    private readonly List<ResolverCreationDefinition> resolverDefinitions = new();

    private readonly List<Diagnostic> diagnostics = new();

    public CompiletimeInjectionDefinitionBuilder(string defaultNamespace)
    {
        this.DefaultNamespace = defaultNamespace;
    }

    public string DefaultNamespace { get; set; }

    public Inject RequiredParameterInjection { get; set; }

    public bool HasBinding(Type type)
    {
        return this.bindingRegistrations.ContainsKey(type.Id);
    }

    public IReadOnlyList<BindingRegistration> TryGetBindingRegistrations(Type type)
    {
        if (this.bindingRegistrations.TryGetValue(type.Id, out var registrations))
        {
            return registrations;
        }

        return Array.Empty<BindingRegistration>();
    }

    public void AddParameter(Type parameterType, Inject inject = Inject.ByType)
    {
        this.AddParameterSource(parameterType, ParameterSource.DirectParameter(inject));
    }

    public void AddPropertyParameter(Type parameterType, AccessorProperty accessorProperty)
    {
        this.AddParameterSource(parameterType, ParameterSource.PropertyAccessorParameter(accessorProperty));
    }

    public void Bind(
        ImmutableArray<(Type Type, TypeMetadata TypeMetadata)> interfaces,
        (Type Type, TypeMetadata TypeMetadata) target,
        Method method,
        Scope? scope = null,
        bool isInjectable = false,
        bool isNewOverridable = false)
    {
        void AddBinding(TypeId typeId, BindingRegistration binding)
        {
            if (!this.bindingRegistrations.TryGetValue(typeId, out var bindingList))
            {
                bindingList = new List<BindingRegistration>();
                this.bindingRegistrations.Add(typeId, bindingList);
            }

            bindingList.Add(binding);
        }

        var targetReferencingType = interfaces.Length > 0 ? interfaces.Last().Type : target.Type;

        var binding = new BindingRegistration(target, targetReferencingType, scope ?? Scope._Auto, method, isInjectable, isNewOverridable);
        AddBinding(target.Type.Id, binding);
        foreach (var @interface in interfaces)
        {
            AddBinding(@interface.Type.Id, binding);
        }
    }

    public void BindGeneric(ImmutableArray<(UnboundGenericType Type, TypeMetadata TypeMetadata)> interfaces, (OpenGenericType Type, TypeMetadata TypeMetadata) implementation, Scope scope, GenericMethod genericMethod)
    {
        void AddBinding(UnboundGenericType type, GenericBindingRegistration genericBinding)
        {
            if (!this.genericBindingRegistrations.TryGetValue(type, out var bindingList))
            {
                bindingList = new List<GenericBindingRegistration>();
                this.genericBindingRegistrations.Add(type, bindingList);
            }

            bindingList.Add(genericBinding);
        }

        var genericBinding = new GenericBindingRegistration(implementation.Type, scope, genericMethod, Injection.Accessibility.Internal, implementation.TypeMetadata.HasLifetime, false);
        AddBinding(implementation.Type.ToUnboundGenericType(), genericBinding);
        foreach (var @interface in interfaces)
        {
            AddBinding(@interface.Type, genericBinding);
        }
    }

    public void CreateFactory(
        FactoryMethodRegistrationBuilder factoryMethodRegistrationBuilder,
        string? factoryClassNamespace = null,
        string? factoryClassName = null,
        bool generateInterface = true,
        Accessibility accessibility = Injection.Accessibility.Public)
    {
        this.factoryDefinitions.Add(new FactoryCreationDefinition(factoryClassNamespace ?? this.DefaultNamespace, factoryClassName, generateInterface, factoryMethodRegistrationBuilder.Build(), accessibility));
    }

    public void CreateResolver(
        FactoryRegistrationBuilder factoryRegistrationBuilder,
        string? resolverClassNamespace = null,
        string? resolverClassName = null,
        bool generateInterface = true,
        Accessibility accessibility = Accessibility.Public)
    {
        this.resolverDefinitions.Add(new ResolverCreationDefinition(resolverClassNamespace ?? this.DefaultNamespace, resolverClassName, generateInterface, factoryRegistrationBuilder.Build(), accessibility));
    }

    public void ReportDiagnostic(Diagnostic diagnostic)
    {
        this.diagnostics.Add(diagnostic);
    }

    public R<InjectionDefinition, ValueList<Diagnostic>> Build()
    {
        if (this.diagnostics.Any())
        {
            return R.Error((ValueList<Diagnostic>)this.diagnostics.ToImmutableList());
        }

        return R.Success(new InjectionDefinition(
            this.DefaultNamespace,
            this.RequiredParameterInjection,
            this.factoryDefinitions.ToImmutableArray(),
            this.bindingRegistrations.ToImmutableDictionary(x => x.Key, x => x.Value.ToImmutableArray().ToValueArray()),
            this.genericBindingRegistrations.ToImmutableDictionary(x => x.Key, x => x.Value.ToImmutableArray().ToValueArray()),
            this.requiredParameters.ToImmutableDictionary(x => x.Key, x => x.Value.ToImmutableArray().ToValueArray()),
            this.resolverDefinitions.ToImmutableArray()));
    }

    private void AddParameterSource(Type parameterType, ParameterSource parameterSource)
    {
        var parameterTypeId = parameterType.Id;
        if (!this.requiredParameters.TryGetValue(parameterTypeId, out var parameterSources))
        {
            parameterSources = new List<ParameterSource>();
            this.requiredParameters.Add(parameterTypeId, parameterSources);
        }

        parameterSources.Add(parameterSource);
    }
}