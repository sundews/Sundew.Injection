// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalysisContextExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using Accessibility = Sundew.Injection.Accessibility;
using ISymbol = Microsoft.CodeAnalysis.ISymbol;
using MethodKind = Sundew.Injection.Generator.TypeSystem.MethodKind;

internal static class AnalysisContextExtensions
{
    private const string Dispose = "Dispose";

    public static void AddFactoryMethodFromTypeSymbol(
        this AnalysisContext analysisContext,
        TypeSymbolWithLocation interfaceTypeSymbolWithLocation,
        TypeSymbolWithLocation implementationTypeSymbolWithLocation,
        Method? factoryMethod,
        string? factoryMethodName,
        Accessibility accessibility,
        bool isNewOverridable,
        FactoryMethodRegistrationBuilder factoryMethodRegistrationBuilder)
    {
        var interfaceTypeResult = analysisContext.TypeFactory.GetFullType(interfaceTypeSymbolWithLocation);
        if (interfaceTypeResult.IsError)
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(interfaceTypeResult.Error);
            return;
        }

        var implementationTypeResult = analysisContext.TypeFactory.GetFullType(implementationTypeSymbolWithLocation);
        if (implementationTypeResult.IsError)
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(implementationTypeResult.Error);
            return;
        }

        if (factoryMethod == null)
        {
            var createMethodResult = GetFactoryTarget(analysisContext, implementationTypeSymbolWithLocation.TypeSymbol);
            if (createMethodResult.TryGetError(out var error, out factoryMethod))
            {
                analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(new ErrorWithLocation(error, implementationTypeSymbolWithLocation.Location));
                return;
            }

            factoryMethod = createMethodResult.Value;
        }

        factoryMethodRegistrationBuilder.Add(
            interfaceTypeResult.Value,
            implementationTypeResult.Value,
            new ScopeContext(Scope._NewInstance(Location.None), ScopeSelection.Implicit),
            factoryMethod,
            factoryMethodName,
            accessibility,
            isNewOverridable);
    }

    public static void AddDefaultFactoryMethodFromTypeSymbol(
        this AnalysisContext analysisContext,
        TypeSymbolWithLocation typeSymbolWithLocation,
        Accessibility accessibility,
        bool isNewOverridable,
        FactoryMethodRegistrationBuilder factoryMethodRegistrationBuilder)
    {
        var interfaceTypeResult = analysisContext.TypeFactory.GetFullType(typeSymbolWithLocation);
        if (interfaceTypeResult.IsError)
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(interfaceTypeResult.Error);
            return;
        }

        var interfaceType = interfaceTypeResult.Value;
        var bindingRegistrations = analysisContext.CompiletimeInjectionDefinitionBuilder.TryGetBindingRegistrations(interfaceType.Type);
        if (bindingRegistrations.Any())
        {
            foreach (var bindingRegistration in bindingRegistrations)
            {
                factoryMethodRegistrationBuilder.Add(
                    interfaceType,
                    bindingRegistration.TargetType,
                    bindingRegistration.Scope,
                    bindingRegistration.Method,
                    default,
                    accessibility,
                    isNewOverridable);
            }
        }
        else
        {
            var typeSymbol = typeSymbolWithLocation.TypeSymbol;
            if (typeSymbol.IsInstantiable() && interfaceType.DefaultConstructor.TryGetValue(out var method))
            {
                analysisContext.CompiletimeInjectionDefinitionBuilder.Bind(ImmutableArray.Create(interfaceType.Type), interfaceType, method, new ScopeContext(Scope._Auto, ScopeSelection.Implicit), false, isNewOverridable);
                var factoryMethodResult = GetFactoryTarget(analysisContext, typeSymbol);
                if (factoryMethodResult.TryGet(out var factoryMethod, out var error))
                {
                    factoryMethodRegistrationBuilder.Add(interfaceType, interfaceType, new ScopeContext(Scope._NewInstance(Location.None), ScopeSelection.Implicit), factoryMethod, default, accessibility, isNewOverridable);
                    return;
                }

                analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(new ErrorWithLocation(error, typeSymbolWithLocation.Location));
            }
            else
            {
                analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.NoBindingFoundForNonConstructableTypeError, typeSymbolWithLocation);
            }
        }
    }

    public static void AddFactoryFromTypeSymbol(
        this AnalysisContext analysisContext,
        TypeSymbolWithLocation factoryTypeSymbolWithLocation,
        FactoryRegistrationBuilder factoryRegistrationBuilder)
    {
        var factoryTypeResult = analysisContext.TypeFactory.GetFullType(factoryTypeSymbolWithLocation);
        if (factoryTypeResult.IsError)
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(factoryTypeResult.Error);
            return;
        }

        factoryRegistrationBuilder.Add(factoryTypeResult.Value.Type, GetFactoryTargets(factoryTypeSymbolWithLocation, analysisContext));
    }

    internal static FactoryMethodTarget? GetFactoryTarget(AnalysisContext analysisContext, ISymbol symbol, Location location)
    {
        switch (symbol)
        {
            case IMethodSymbol methodSymbol:
                if (methodSymbol.MethodKind == Microsoft.CodeAnalysis.MethodKind.DeclareMethod
                    && symbol.GetAttributes().All(x => x.AttributeClass?.ToDisplayString() != KnownTypesProvider.IndirectFactoryTargetName)
                    && !symbol.MetadataName.Contains(Dispose))
                {
                    var factoryMethodTargetResult = analysisContext.TypeFactory.GetFactoryMethodTarget(methodSymbol);
                    if (factoryMethodTargetResult.TryGet(out var factoryMethodTarget, out var error))
                    {
                        return factoryMethodTarget;
                    }

                    analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(new ErrorWithLocation(error, location));
                }

                break;
            case IPropertySymbol propertySymbol:
                if (propertySymbol.GetMethod != default
                    && symbol.GetAttributes().All(x => x.AttributeClass?.ToDisplayString() != KnownTypesProvider.IndirectFactoryTargetName)
                    && !symbol.MetadataName.Contains(Dispose))
                {
                    var factoryMethodTargetResult = analysisContext.TypeFactory.GetFactoryMethodTarget(propertySymbol);
                    if (factoryMethodTargetResult.TryGet(out var factoryMethodTarget, out var error))
                    {
                        return factoryMethodTarget;
                    }

                    analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(new ErrorWithLocation(error, location));
                }

                break;
        }

        return default;
    }

    private static ValueArray<FactoryMethodTarget> GetFactoryTargets(TypeSymbolWithLocation factoryTypeSymbol, AnalysisContext analysisContext)
    {
        return factoryTypeSymbol.TypeSymbol.GetMembers()
            .Select(symbol => GetFactoryTarget(analysisContext, symbol, factoryTypeSymbol.Location))
            .WhereNotDefault()
            .ToValueArray();
    }

    private static R<Method, Error> GetFactoryTarget(AnalysisContext analysisContext, ITypeSymbol implementationType)
    {
        var defaultConstructor = TypeHelper.GetDefaultConstructorMethod(implementationType);
        if (defaultConstructor != null)
        {
            return analysisContext.TypeFactory.GetFactoryMethod(defaultConstructor);
        }

        return R.Success(
            new Method(
                analysisContext.TypeFactory.GetType(implementationType),
                implementationType.MetadataName,
                MethodKind._Constructor));
    }
}