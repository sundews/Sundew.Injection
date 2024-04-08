// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImplementServiceProviderVisitor.cs" company="Sundews">
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

internal class ImplementServiceProviderVisitor(
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
        var factories = new FactoryRegistrationBuilder();
        var accessibility = parameters[i++].ExplicitDefaultValue.ToEnumOrDefault(Injection.Accessibility.Public);
        var argumentIndex = 0;
        foreach (var argumentSyntax in node.Arguments)
        {
            if (argumentSyntax.NameColon != null)
            {
                switch (argumentSyntax.NameColon.Name.ToString())
                {
                    case nameof(factories):
                        new FactoryVisitor(factories, analysisContext).Visit(argumentSyntax);
                        break;
                    case nameof(accessibility):
                        accessibility = analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value.ToEnumOrDefault(Injection.Accessibility.Public);
                        break;
                }
            }
            else
            {
                switch (argumentIndex)
                {
                    case 0:
                        new FactoryVisitor(factories, analysisContext).Visit(argumentSyntax);
                        break;
                    case 1:
                        accessibility = analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value.ToEnumOrDefault(Injection.Accessibility.Public);
                        break;
                }

                argumentIndex++;
            }
        }

        var typeArguments = methodSymbol.MapTypeArguments(genericNameSyntax);
        var resolverTypeResult = analysisContext.TypeFactory.GetNamedType(typeArguments[0]);
        if (resolverTypeResult.TryGetError(out var invalidFactoryTypeSymbol))
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.InvalidFactoryTypeError, invalidFactoryTypeSymbol);
            return;
        }

        analysisContext.CompiletimeInjectionDefinitionBuilder.CreateResolver(factories, resolverTypeResult.Value, accessibility, location);
    }
}