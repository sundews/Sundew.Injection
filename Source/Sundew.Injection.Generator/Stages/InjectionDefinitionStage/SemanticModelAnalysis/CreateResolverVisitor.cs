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
        var i = 2;
        var factories = new FactoryRegistrationBuilder();
        string? resolverName = null;
        var generateInterface = (bool?)parameters[i++].ExplicitDefaultValue ?? true;
        var accessibility = parameters[i++].ExplicitDefaultValue.ToEnumOrDefault(Injection.Accessibility.Public);
        var @namespace = (string?)parameters[i++].ExplicitDefaultValue;
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
                    case nameof(resolverName):
                        resolverName = (string?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value;
                        break;
                    case nameof(generateInterface):
                        generateInterface = (bool?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value ?? true;
                        break;
                    case nameof(accessibility):
                        accessibility = this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value.ToEnumOrDefault(Injection.Accessibility.Public);
                        break;
                    case nameof(@namespace):
                        @namespace = (string?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value;
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
                        resolverName = (string?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value;
                        break;
                    case 2:
                        generateInterface = (bool?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value ?? true;
                        break;
                    case 3:
                        accessibility = this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value.ToEnumOrDefault(Injection.Accessibility.Public);
                        break;
                    case 4:
                        @namespace = (string?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value;
                        break;
                }

                argumentIndex++;
            }
        }

        this.analysisContext.CompiletimeInjectionDefinitionBuilder.CreateResolver(factories, @namespace, resolverName, generateInterface, accessibility);
    }
}