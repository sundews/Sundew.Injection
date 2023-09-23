﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalysisContextExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Base.Primitives.Computation;
using Sundew.Injection.Generator.TypeSystem;
using Accessibility = Sundew.Injection.Accessibility;
using MethodKind = Sundew.Injection.Generator.TypeSystem.MethodKind;

internal static class AnalysisContextExtensions
{
    public static void AddFactoryMethodFromTypeSymbol(
        this AnalysisContext analysisContext,
        ITypeSymbol interfaceTypeSymbol,
        ITypeSymbol implementationTypeSymbol,
        Method? createMethod,
        string? createMethodName,
        Accessibility accessibility,
        bool isNewOverridable,
        FactoryMethodRegistrationBuilder factoryMethodRegistrationBuilder)
    {
        var interfaceType = analysisContext.TypeFactory.CreateType(interfaceTypeSymbol);
        var implementationType = analysisContext.TypeFactory.CreateType(implementationTypeSymbol);
        factoryMethodRegistrationBuilder.Add(interfaceType, implementationType, Scope._NewInstance, createMethod ?? CreateMethod(analysisContext, implementationTypeSymbol), createMethodName, accessibility, isNewOverridable);
    }

    public static void AddDefaultFactoryMethodFromTypeSymbol(
        this AnalysisContext analysisContext,
        ITypeSymbol typeSymbol,
        Accessibility accessibility,
        bool isNewOverridable,
        FactoryMethodRegistrationBuilder factoryMethodRegistrationBuilder)
    {
        var interfaceType = analysisContext.TypeFactory.CreateType(typeSymbol);

        var bindingRegistrations = analysisContext.CompiletimeInjectionDefinitionBuilder.TryGetBindingRegistrations(interfaceType.Type);
        if (bindingRegistrations.Any())
        {
            foreach (var bindingRegistration in bindingRegistrations)
            {
                factoryMethodRegistrationBuilder.Add(interfaceType, bindingRegistration.TargetType, bindingRegistration.Scope, bindingRegistration.Method, null, accessibility, isNewOverridable);
            }
        }
        else
        {
            if (typeSymbol.IsInstantiable() && interfaceType.TypeMetadata.DefaultConstructor.TryGetValue(out var method))
            {
                analysisContext.CompiletimeInjectionDefinitionBuilder.Bind(ImmutableArray.Create(interfaceType), interfaceType, method, TypeSystem.Scope._Auto, false, isNewOverridable);
                var type = analysisContext.TypeFactory.CreateType(typeSymbol);
                factoryMethodRegistrationBuilder.Add(type, type, Scope._NewInstance, CreateMethod(analysisContext, typeSymbol), null, accessibility, isNewOverridable);
            }
            else
            {
                analysisContext.CompiletimeInjectionDefinitionBuilder.ReportDiagnostic(Diagnostic.Create(Diagnostics.NoBindingFoundForNonConstructableTypeError, null, typeSymbol.ToDisplayString()));
            }
        }
    }

    private static Method CreateMethod(AnalysisContext analysisContext, ITypeSymbol implementationType)
    {
        var defaultConstructor = TypeHelper.GetDefaultConstructorMethod(implementationType);
        if (defaultConstructor != null)
        {
            return analysisContext.TypeFactory.CreateMethod(defaultConstructor);
        }

        return new Method(ImmutableArray<Parameter>.Empty, implementationType.MetadataName, analysisContext.TypeFactory.CreateType(implementationType).Type, MethodKind._Constructor);
    }
}