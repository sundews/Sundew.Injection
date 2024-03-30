// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateResolverVisitor.cs" company="Sundews">
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

internal class CreateResolverVisitor : CSharpSyntaxWalker
{
    private readonly IMethodSymbol methodSymbol;
    private readonly AnalysisContext analysisContext;

    public CreateResolverVisitor(IMethodSymbol methodSymbol, AnalysisContext analysisContext)
    {
        this.methodSymbol = methodSymbol;
        this.analysisContext = analysisContext;
    }

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        var parameters = this.methodSymbol.Parameters;
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
                        new FactoryVisitor(factories, this.analysisContext).Visit(argumentSyntax);
                        break;
                    case nameof(accessibility):
                        accessibility = this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value.ToEnumOrDefault(Injection.Accessibility.Public);
                        break;
                }
            }
            else
            {
                switch (argumentIndex)
                {
                    case 0:
                        new FactoryVisitor(factories, this.analysisContext).Visit(argumentSyntax);
                        break;
                    case 1:
                        accessibility = this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value.ToEnumOrDefault(Injection.Accessibility.Public);
                        break;
                }

                argumentIndex++;
            }
        }

        var typeArguments = this.methodSymbol.TypeArguments;
        var resolverTypeResult = this.analysisContext.TypeFactory.GetType(typeArguments[0]);
        if (resolverTypeResult.TryGetError(out var factoryTypeError))
        {
            this.analysisContext.CompiletimeInjectionDefinitionBuilder.ReportDiagnostic(Diagnostic.Create(Diagnostics.InvalidFactoryTypeError, Location.None, factoryTypeError));
            return;
        }

        this.analysisContext.CompiletimeInjectionDefinitionBuilder.CreateResolver(factories, resolverTypeResult.Value, accessibility);
    }
}