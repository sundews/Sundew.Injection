﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateFactoryVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using Accessibility = Sundew.Injection.Accessibility;

internal class CreateFactoryVisitor(
    IMethodSymbol methodSymbol,
    AnalysisContext analysisContext)
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

        var typeArguments = methodSymbol.TypeArguments;
        var factoryType = analysisContext.TypeFactory.GetType(typeArguments[0]);
        R<NamedType, string>? factoryInterfaceTypeResult = typeArguments.Length == 2 ? analysisContext.TypeFactory.GetType(typeArguments[1]) : null;
        if (factoryType.TryGetError(out var factoryTypeError))
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.ReportDiagnostic(Diagnostic.Create(Diagnostics.InvalidFactoryTypeError, Location.None, factoryTypeError));
            return;
        }

        NamedType? factoryInterfaceType = default;
        if (factoryInterfaceTypeResult.TryGetValue(out var result) && !result.TryGet(out factoryInterfaceType, out var factoryInterfaceTypeError))
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.ReportDiagnostic(Diagnostic.Create(Diagnostics.InvalidFactoryTypeError, Location.None, factoryInterfaceTypeError));
            return;
        }

        analysisContext.CompiletimeInjectionDefinitionBuilder.CreateFactory(factoryType.Value, factoryInterfaceType, factoryMethods, accessibility);
    }
}