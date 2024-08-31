// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImplementFactoryVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base;
using Sundew.Base.Collections;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using Accessibility = Sundew.Injection.Accessibility;
using ISymbol = Microsoft.CodeAnalysis.ISymbol;

internal class ImplementFactoryVisitor(
    GenericNameSyntax genericNameSyntax,
    IMethodSymbol methodSymbol,
    AnalysisContext analysisContext,
    Location location)
    : CSharpSyntaxWalker
{
    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        var parameters = methodSymbol.Parameters;
        var i = 1;
        var factoryMethods = new FactoryMethodRegistrationBuilder();
        var accessibilityParameter = parameters[i++];
        var accessibility = accessibilityParameter.HasExplicitDefaultValue ? accessibilityParameter.ExplicitDefaultValue?.ToEnumOrDefault(Accessibility.Public) ?? Accessibility.Public : Accessibility.Public;
        var argumentIndex = 0;
        foreach (var argumentSyntax in node.Arguments)
        {
            if (argumentSyntax.NameColon != null)
            {
                switch (argumentSyntax.NameColon.Name.ToString())
                {
                    case nameof(factoryMethods):
                        new FactoryMethodVisitor(factoryMethods, analysisContext).Visit(argumentSyntax);
                        break;
                    case nameof(accessibility):
                        accessibility = analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value.ToEnumOrDefault(Accessibility.Public);
                        break;
                }
            }
            else
            {
                switch (argumentIndex)
                {
                    case 0:
                        new FactoryMethodVisitor(factoryMethods, analysisContext).Visit(argumentSyntax);
                        break;
                    case 1:
                        accessibility = analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value.ToEnumOrDefault(Accessibility.Public);
                        break;
                }

                argumentIndex++;
            }
        }

        var typeArguments = methodSymbol.MapTypeArguments(genericNameSyntax);
        var factoryTypeSymbol = typeArguments[0];
        var factoryType = analysisContext.TypeFactory.GetNamedType(factoryTypeSymbol);
        TypeSymbolWithLocation? factoryInterfaceTypeSymbol = typeArguments.Length == 2 ? typeArguments[1] : null;
        R<NamedType, TypeSymbolWithLocation>? factoryInterfaceTypeResult = factoryInterfaceTypeSymbol.HasValue ? analysisContext.TypeFactory.GetNamedType(factoryInterfaceTypeSymbol.Value) : null;
        this.GetFactoryMethods(factoryMethods, factoryTypeSymbol, factoryInterfaceTypeSymbol);
        if (factoryType.TryGetError(out var invalidFactoryTypeSymbol))
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.InvalidFactoryTypeError, invalidFactoryTypeSymbol);
            return;
        }

        NamedType? factoryInterfaceType = default;
        if (factoryInterfaceTypeResult.TryGetValue(out var result)
            && !result.TryGet(out factoryInterfaceType, out var invalidFactoryInterfaceTypeSymbol))
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.InvalidFactoryTypeError, invalidFactoryInterfaceTypeSymbol);
            return;
        }

        analysisContext.CompiletimeInjectionDefinitionBuilder.ImplementFactory(factoryType.Value, factoryInterfaceType, factoryMethods, accessibility, location);
    }

    internal static R<Method, SymbolError>? GetFactoryTarget(AnalysisContext analysisContext, ISymbol symbol)
    {
        const string dispose = "Dispose";
        switch (symbol)
        {
            case IMethodSymbol methodSymbol:
                if (methodSymbol.MethodKind == Microsoft.CodeAnalysis.MethodKind.DeclareMethod
                    && symbol.GetAttributes().All(x => x.AttributeClass?.ToDisplayString() != KnownTypesProvider.IndirectFactoryTargetName)
                    && !symbol.MetadataName.Contains(dispose))
                {
                    return analysisContext.TypeFactory.GetFactoryMethod(methodSymbol);
                }

                break;
            case IPropertySymbol propertySymbol:
                if (propertySymbol.GetMethod != default
                    && symbol.GetAttributes().All(x => x.AttributeClass?.ToDisplayString() != KnownTypesProvider.IndirectFactoryTargetName)
                    && !symbol.MetadataName.Contains(dispose))
                {
                    return analysisContext.TypeFactory.GetFactoryMethod(propertySymbol).ToResult(() => new SymbolError(new NamedSymbol(propertySymbol.ToDisplayString()), Array.Empty<SymbolError>()));
                }

                break;
        }

        return default;
    }

    private void GetFactoryMethods(FactoryMethodRegistrationBuilder factoryMethodRegistrationBuilder, TypeSymbolWithLocation factoryTypeSymbol, TypeSymbolWithLocation? factoryInterfaceTypeSymbol)
    {
        var factories = (factory: factoryTypeSymbol.TypeSymbol, members: factoryTypeSymbol.TypeSymbol.GetMembers()).ToEnumerable().Concat(factoryTypeSymbol.TypeSymbol.AllInterfaces.Select(x => (factory: (ITypeSymbol)x, members: x.GetMembers())));
        foreach (var factory in factories)
        {
            foreach (var member in factory.members)
            {
                switch (member)
                {
                    case IMethodSymbol methodSymbol:
                        var containingType = analysisContext.TypeFactory.GetType(methodSymbol.ContainingType);
                        var returnTypeResult = analysisContext.TypeFactory.GetFullType(methodSymbol.ReturnType);
                        if (!returnTypeResult.IsError)
                        {
                        }
                        else
                        {
                            var returnType = returnTypeResult.Value;
                            var bindingRegistrations = analysisContext.CompiletimeInjectionDefinitionBuilder.TryGetBindingRegistrations(returnTypeResult.Value.Type);
                            if (bindingRegistrations.Any())
                            {
                                foreach (var bindingRegistration in bindingRegistrations)
                                {
                                    factoryMethodRegistrationBuilder.Add(
                                        returnTypeResult.Value,
                                        bindingRegistration.TargetType,
                                        bindingRegistration.Scope,
                                        bindingRegistration.Method,
                                        default,
                                        Accessibility.Public,
                                        false);
                                }
                            }
                            else
                            {
                                var typeSymbol = methodSymbol.ReturnType;
                                if (typeSymbol.IsInstantiable() && returnType.DefaultConstructor.TryGetValue(out var method))
                                {
                                    analysisContext.CompiletimeInjectionDefinitionBuilder.Bind(ImmutableArray.Create(returnType.Type), returnType, method, new ScopeContext(Scope._Auto, ScopeSelection.Implicit), false, false);
                                    var createMethodResult = GetFactoryTarget(analysisContext, typeSymbol);
                                    if (!createMethodResult.HasValue)
                                    {
                                        // analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.InfiniteRecursionError, new SymbolErrorWithLocation(createMethodResult.Error, typeSymbolWithLocation.Location));
                                        return;
                                    }

                                    if (createMethodResult.Value.TryGetValue(out var factoryMethod))
                                    {
                                        factoryMethodRegistrationBuilder.Add(returnType, returnType, new ScopeContext(Scope._NewInstance(Location.None), ScopeSelection.Implicit), factoryMethod, default, typeSymbol.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Public ? Accessibility.Public : Accessibility.Internal, true);
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }
                                else
                                {
                                    analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.NoBindingFoundForNonConstructableTypeError, factoryTypeSymbol);
                                }
                            }
                        }

                        break;
                    case IPropertySymbol propertySymbol:
                        break;
                }
            }
        }
    }
}