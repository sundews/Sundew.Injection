// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompiletimeInjectionDefinitionBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal sealed class CompiletimeInjectionDefinitionBuilder : IInjectionDefinitionBuilder
{
    private readonly Dictionary<Type, List<BindingRegistration>> bindingRegistrations = new();

    private readonly Dictionary<UnboundGenericType, List<GenericBindingRegistration>> genericBindingRegistrations = new();

    private readonly Dictionary<Type, List<ParameterSource>> requiredParameters = new();

    private readonly List<FactoryCreationDefinition> factoryDefinitions = new();

    public CompiletimeInjectionDefinitionBuilder(string defaultNamespace)
    {
        this.DefaultNamespace = defaultNamespace;
    }

    public string DefaultNamespace { get; set; }

    public Inject RequiredParameterInjection { get; set; }

    public void AddParameter(Type parameterType, Inject inject = Inject.ByType)
    {
        this.AddParameterSource(parameterType, ParameterSource.DirectParameter(inject));
    }

    public void AddPropertyParameter(Type parameterType, AccessorProperty accessorProperty)
    {
        this.AddParameterSource(parameterType, ParameterSource.PropertyAccessorParameter(accessorProperty));
    }

    public void Bind(ImmutableArray<(UnboundGenericType Type, TypeMetadata TypeMetadata)> interfaces, (GenericType Type, TypeMetadata TypeMetadata) implementation, Scope scope, GenericMethod genericMethod)
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

        var genericBinding = new GenericBindingRegistration(implementation.Type, scope, genericMethod, implementation.TypeMetadata.ImplementsIDisposable, Accessibility.Internal, false);
        AddBinding(implementation.Type.ToUnboundGenericType(), genericBinding);
        foreach (var @interface in interfaces)
        {
            AddBinding(@interface.Type, genericBinding);
        }
    }

    public void Bind(
        ImmutableArray<(Type Type, TypeMetadata Metadata)> interfaces,
        (Type Type, TypeMetadata Metadata) implementation,
        Method method,
        Scope? scope = null,
        bool isInjectable = false,
        bool isNewOverridable = false)
    {
        void AddBinding(Type type, BindingRegistration binding)
        {
            if (!this.bindingRegistrations.TryGetValue(type, out var bindingList))
            {
                bindingList = new List<BindingRegistration>();
                this.bindingRegistrations.Add(type, bindingList);
            }

            bindingList.Add(binding);
        }

        var commonType = interfaces.Length > 0 ? interfaces.Last().Type : implementation.Type;

        var binding = new BindingRegistration(implementation.Type, commonType, scope ?? Scope.Auto, method, implementation.Metadata.ImplementsIDisposable, isInjectable, isNewOverridable);
        AddBinding(implementation.Type, binding);
        foreach (var @interface in interfaces)
        {
            AddBinding(@interface.Type, binding);
        }
    }

    public void CreateFactory(
        FactoryMethodRegistrationBuilder factoryMethodRegistrationBuilder,
        string? factoryClassNamespace = null,
        string? factoryClassName = null,
        bool generateInterface = true,
        Injection.Accessibility accessibility = Injection.Accessibility.Public)
    {
        this.factoryDefinitions.Add(new FactoryCreationDefinition(factoryClassNamespace ?? this.DefaultNamespace, factoryClassName, generateInterface, factoryMethodRegistrationBuilder.Build(), accessibility));
    }

    public InjectionDefinition Build()
    {
        return new InjectionDefinition(
            this.DefaultNamespace,
            this.RequiredParameterInjection,
            this.factoryDefinitions.ToImmutableArray(),
            this.bindingRegistrations.ToImmutableDictionary(x => x.Key, x => x.Value.ToImmutableArray().ToValueArray()),
            this.genericBindingRegistrations.ToImmutableDictionary(x => x.Key, x => x.Value.ToImmutableArray().ToValueArray()),
            this.requiredParameters.ToImmutableDictionary(x => x.Key, x => x.Value.ToImmutableArray().ToValueArray()));
    }

    private void AddParameterSource(Type parameterType, ParameterSource parameterSource)
    {
        if (!this.requiredParameters.TryGetValue(parameterType, out var parameterSources))
        {
            parameterSources = new List<ParameterSource>();
            this.requiredParameters.Add(parameterType, parameterSources);
        }

        parameterSources.Add(parameterSource);
    }
}