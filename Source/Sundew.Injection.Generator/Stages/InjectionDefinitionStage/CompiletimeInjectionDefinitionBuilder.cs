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
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;
using Sundew.Injection.Generator.TypeSystem;
using Accessibility = Sundew.Injection.Accessibility;

internal sealed class CompiletimeInjectionDefinitionBuilder : IInjectionDefinitionBuilder
{
    private readonly Dictionary<TypeId, List<BindingRegistration>> bindingRegistrations = [];

    private readonly Dictionary<UnboundGenericType, List<GenericBindingRegistration>> genericBindingRegistrations = [];

    private readonly Dictionary<TypeId, List<ParameterSource>> requiredParameterSources = [];

    private readonly Dictionary<TypeId, ScopeContext> requiredParameterScopes = [];

    private readonly List<FactoryCreationDefinition> factoryDefinitions = [];

    private readonly List<ResolverCreationDefinition> resolverDefinitions = [];

    private readonly List<Diagnostic> diagnostics = [];

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

        return [];
    }

    public void AddParameter(Type parameterType, Inject inject = Inject.Shared, ScopeContext scope = default)
    {
        this.AddParameterSource(parameterType, ParameterSource.DirectParameter(inject));
        this.AddParameterScope(parameterType, scope);
    }

    public void AddPropertyParameter(Type parameterType, AccessorProperty accessorProperty, bool needsInvocation, ScopeContext scope = default)
    {
        this.AddParameterSource(parameterType, ParameterSource.PropertyAccessorParameter(accessorProperty, needsInvocation));
        this.AddParameterScope(parameterType, scope);
    }

    public void Bind(
        ImmutableArray<Type> interfaces,
        FullType target,
        Method method,
        ScopeContext? scope = null,
        bool isInjectable = false,
        bool isNewOverridable = false)
    {
        void AddBinding(TypeId typeId, BindingRegistration binding)
        {
            if (!this.bindingRegistrations.TryGetValue(typeId, out var bindingList))
            {
                bindingList = [];
                this.bindingRegistrations.Add(typeId, bindingList);
            }

            bindingList.Add(binding);
        }

        var targetReferencingType = interfaces.Length > 0 ? interfaces.Last() : target.Type;

        var bindingRegistration = new BindingRegistration(target, targetReferencingType, scope ?? new ScopeContext(Scope._Auto, ScopeSelection.Implicit), method, isInjectable, isNewOverridable);
        AddBinding(target.Type.Id, bindingRegistration);
        foreach (var @interface in interfaces)
        {
            AddBinding(@interface.Id, bindingRegistration);
        }
    }

    public void BindGeneric(ImmutableArray<(UnboundGenericType Type, TypeMetadata TypeMetadata)> interfaces, (OpenGenericType Type, TypeMetadata TypeMetadata) implementation, ScopeContext scope, GenericMethod genericMethod)
    {
        void AddBinding(UnboundGenericType type, GenericBindingRegistration genericBinding)
        {
            if (!this.genericBindingRegistrations.TryGetValue(type, out var bindingList))
            {
                bindingList = [];
                this.genericBindingRegistrations.Add(type, bindingList);
            }

            bindingList.Add(genericBinding);
        }

        var genericBinding = new GenericBindingRegistration(implementation.Type, scope, genericMethod, Injection.Accessibility.Internal, implementation.TypeMetadata.HasLifecycle, false);
        AddBinding(implementation.Type.ToUnboundGenericType(), genericBinding);
        foreach (var @interface in interfaces)
        {
            AddBinding(@interface.Type, genericBinding);
        }
    }

    public void CreateFactory(
        NamedType factoryType,
        NamedType? factoryInterface,
        FactoryMethodRegistrationBuilder factoryMethodRegistrationBuilder,
        Accessibility accessibility,
        Location location)
    {
        this.factoryDefinitions.Add(new FactoryCreationDefinition(factoryType, factoryInterface, factoryMethodRegistrationBuilder.Build(), accessibility, location));
    }

    public void CreateResolver(
        FactoryRegistrationBuilder factoryRegistrationBuilder,
        NamedType resolverType,
        Accessibility accessibility,
        Location location)
    {
        this.resolverDefinitions.Add(new ResolverCreationDefinition(resolverType, factoryRegistrationBuilder.Build(), accessibility, location));
    }

    public void AddDiagnostics(IEnumerable<Diagnostic> diagnostics)
    {
        foreach (var diagnostic in diagnostics)
        {
            this.diagnostics.Add(diagnostic);
        }
    }

    public void AddDiagnostic(Diagnostic diagnostic)
    {
        this.diagnostics.Add(diagnostic);
    }

    public void AddDiagnostic(DiagnosticDescriptor diagnosticDescriptor, SymbolErrorWithLocation symbolErrorWithLocation, params object[] additionalArguments)
    {
        foreach (var diagnostic in Diagnostics.Create(diagnosticDescriptor, symbolErrorWithLocation, additionalArguments))
        {
            this.AddDiagnostic(diagnostic);
        }
    }

    public void AddDiagnostic(DiagnosticDescriptor diagnosticDescriptor, TypeSymbolWithLocation typeSymbolWithLocation, params object[] additionalArguments)
    {
        this.AddDiagnostic(diagnosticDescriptor, typeSymbolWithLocation.TypeSymbol, typeSymbolWithLocation.Location, additionalArguments);
    }

    public void AddDiagnostic(DiagnosticDescriptor diagnosticDescriptor, Microsoft.CodeAnalysis.ISymbol symbol, Location? location = default, params object[] additionalArguments)
    {
        foreach (var diagnostic in Diagnostics.Create(diagnosticDescriptor, symbol, location, additionalArguments))
        {
            this.AddDiagnostic(diagnostic);
        }
    }

    public R<InjectionDefinition, Diagnostics> Build()
    {
        if (this.diagnostics.Any())
        {
            return R.Error(new Diagnostics(this.diagnostics.ToImmutableList()));
        }

        return R.Success(new InjectionDefinition(
            this.RequiredParameterInjection,
            this.factoryDefinitions.ToImmutableArray(),
            this.bindingRegistrations.ToImmutableDictionary(x => x.Key, x => x.Value.ToImmutableArray().ToValueArray()),
            this.genericBindingRegistrations.ToImmutableDictionary(x => x.Key, x => x.Value.ToImmutableArray().ToValueArray()),
            this.requiredParameterSources.ToImmutableDictionary(x => x.Key, x => x.Value.ToImmutableArray().ToValueArray()),
            this.requiredParameterScopes.ToImmutableDictionary(),
            this.resolverDefinitions.ToImmutableArray()));
    }

    private void AddParameterSource(Type parameterType, ParameterSource parameterSource)
    {
        var parameterTypeId = parameterType.Id;
        if (!this.requiredParameterSources.TryGetValue(parameterTypeId, out var parameterSources))
        {
            parameterSources = [];
            this.requiredParameterSources.Add(parameterTypeId, parameterSources);
        }

        parameterSources.Add(parameterSource);
    }

    private void AddParameterScope(Type parameterType, ScopeContext? scope)
    {
        this.requiredParameterScopes[parameterType.Id] = scope ?? new ScopeContext(Scope._SingleInstancePerRequest(Location.None), ScopeSelection.Implicit);
    }
}