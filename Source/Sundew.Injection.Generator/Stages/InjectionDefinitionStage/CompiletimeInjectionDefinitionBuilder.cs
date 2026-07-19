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
using Sundew.Base.Collections.Linq;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;
using Sundew.Injection.Generator.TypeSystem;
using Accessibility = Sundew.Injection.Accessibility;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

internal sealed class CompiletimeInjectionDefinitionBuilder : IInjectionDefinitionBuilder
{
    private readonly Dictionary<TypeId, List<BindingRegistration>> bindingRegistrations = [];

    private readonly Dictionary<UnboundGenericType, List<GenericBindingRegistration>> genericBindingRegistrations = [];

    private readonly List<FactoryImplementationDefinition> factoryDefinitions = [];

    private readonly List<ServiceProviderImplementationDefinition> serviceProviderImplementationDefinitions = [];

    private readonly List<Diagnostic> diagnostics = [];

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

    public void Bind(
        ImmutableArray<Type> interfaces,
        FullType target,
        Method method,
        ScopeContext? scope = null,
        bool isInjectable = false,
        bool isNewOverridable = false)
    {
        Bind(interfaces, target, method, scope, isInjectable, isNewOverridable, this.bindingRegistrations);
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

        var genericBinding = new GenericBindingRegistration(implementation.Type, scope, genericMethod, Injection.Accessibility.Internal, implementation.TypeMetadata.Lifecycle, false);
        AddBinding(implementation.Type.ToUnboundGenericType(), genericBinding);
        foreach (var @interface in interfaces)
        {
            AddBinding(@interface.Type, genericBinding);
        }
    }

    public void ImplementFactory(
        NamedType factoryType,
        NamedType? factoryInterface,
        DeclaredConstructor declaredConstructor,
        ValueDictionary<TypeId, ParameterSourceContexts> constructorParameterSourceContexts,
        FactoryMethodRegistrationBuilder factoryMethodRegistrationBuilder,
        Accessibility accessibility,
        Location location)
    {
        this.factoryDefinitions.Add(new FactoryImplementationDefinition(factoryType, factoryInterface, declaredConstructor, constructorParameterSourceContexts, factoryMethodRegistrationBuilder.Build(), accessibility, location));
    }

    public void ImplementServiceProvider(
        FactoryRegistrationBuilder factoryRegistrationBuilder,
        NamedType serviceProviderType,
        Accessibility accessibility,
        Location location)
    {
        this.serviceProviderImplementationDefinitions.Add(new ServiceProviderImplementationDefinition(serviceProviderType, factoryRegistrationBuilder.Build(), accessibility, location));
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

    public void AddDiagnostic(ErrorWithLocation errorWithLocation, params object[] additionalArguments)
    {
        foreach (var diagnostic in Diagnostics.Create(errorWithLocation, additionalArguments))
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

    public R<InjectionDefinition, Diagnostics> Build(AnalysisContext analysisContext)
    {
        var lifecycleParameterResult = analysisContext.TypeFactory.GetFullType(analysisContext.KnownAnalysisTypes.LifecycleParameters);
        if (lifecycleParameterResult.TryGetError(out var error, out var lifecycleParameter))
        {
            this.AddDiagnostic(Diagnostic.Create(Diagnostics.RequiredTypeNotFoundError, default, error));
        }

        if (lifecycleParameter.DefaultConstructor == default)
        {
            this.AddDiagnostic(Diagnostic.Create(Diagnostics.NoFactoryMethodFoundForTypeError, default, analysisContext.KnownAnalysisTypes.LifecycleParameters.ToDisplayString()));
        }

        var initializationParameter = analysisContext.TypeFactory.GetType(analysisContext.KnownAnalysisTypes.InitializationParameters);
        var disposalParameter = analysisContext.TypeFactory.GetType(analysisContext.KnownAnalysisTypes.DisposalParameters);
        var ilifecycleParameter = analysisContext.TypeFactory.GetType(analysisContext.KnownAnalysisTypes.ILifecycleParameters);
        var result = new[] { initializationParameter, disposalParameter, ilifecycleParameter }.AllOrFailed(x => x.ToItem());
        if (result.TryGetError(out var errors, out var interfaceTypes))
        {
            this.AddDiagnostics(errors.Items.Select(x => Diagnostic.Create(Diagnostics.RequiredTypeNotFoundError, default, x.Item)));
        }

        if (this.diagnostics.Any())
        {
            return R.Error(new Diagnostics(this.diagnostics.ToImmutableList()));
        }

        Dictionary<TypeId, List<BindingRegistration>> fallbackBindingRegistrations = [];
        Bind(interfaceTypes.Items.ToImmutableArray(), lifecycleParameter, lifecycleParameterResult.Value.DefaultConstructor!, new ScopeContext(Scope._Auto, ScopeSelection.Default), false, false, fallbackBindingRegistrations);

        return R.Success(new InjectionDefinition(
            this.factoryDefinitions.ToImmutableArray(),
            fallbackBindingRegistrations.ToImmutableDictionary(x => x.Key, x => x.Value.ToValueArray()),
            this.bindingRegistrations.ToImmutableDictionary(x => x.Key, x => x.Value.ToValueArray()),
            this.genericBindingRegistrations.ToImmutableDictionary(x => x.Key, x => x.Value.ToValueArray()),
            this.serviceProviderImplementationDefinitions.ToImmutableArray()));
    }

    private static void Bind(
        ImmutableArray<Type> interfaces,
        FullType target,
        Method method,
        ScopeContext? scope,
        bool isInjectable,
        bool isNewOverridable,
        Dictionary<TypeId, List<BindingRegistration>> bindings)
    {
        void AddBinding(TypeId typeId, BindingRegistration binding)
        {
            if (!bindings.TryGetValue(typeId, out var bindingList))
            {
                bindingList = [];
                bindings.Add(typeId, bindingList);
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
}