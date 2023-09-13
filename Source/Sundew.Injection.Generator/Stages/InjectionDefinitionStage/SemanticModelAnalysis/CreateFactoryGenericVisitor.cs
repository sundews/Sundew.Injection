// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateFactoryGenericVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base.Primitives;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

internal class CreateFactoryGenericVisitor : CSharpSyntaxWalker
{
    private readonly IMethodSymbol methodSymbol;
    private readonly AnalysisContext analysisContext;

    public CreateFactoryGenericVisitor(IMethodSymbol methodSymbol, AnalysisContext analysisContext)
    {
        this.methodSymbol = methodSymbol;
        this.analysisContext = analysisContext;
    }

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        var typeArguments = this.methodSymbol.TypeArguments;
        var parameters = this.methodSymbol.Parameters;
        var factoryMethods = new FactoryMethodRegistrationBuilder(this.analysisContext.TypeFactory);
        var i = 0;
        var factoryName = (string?)parameters[i++].ExplicitDefaultValue;
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
                    case nameof(@namespace):
                        @namespace = (string?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value;
                        break;
                    case nameof(factoryName):
                        factoryName = (string?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value;
                        break;
                    case nameof(generateInterface):
                        generateInterface = (bool?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value ?? true;
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
                        factoryName = (string?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value;
                        break;
                    case 1:
                        generateInterface = (bool?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value ?? true;
                        break;
                    case 2:
                        accessibility = this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value.ToEnumOrDefault(Injection.Accessibility.Public);
                        break;
                    case 3:
                        @namespace = (string?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value;
                        break;
                }

                argumentIndex++;
            }
        }

        var implementationType = typeArguments.Last();
        factoryMethods.Add(implementationType, implementationType, null, null, accessibility, false);

        this.analysisContext.CompiletimeInjectionDefinitionBuilder.CreateFactory(factoryMethods, @namespace, factoryName, generateInterface, accessibility);
    }

    public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        var symbolInfo = this.analysisContext.SemanticModel.GetSymbolInfo(node);
        if (symbolInfo.Symbol != null)
        {
            switch (symbolInfo.Symbol.Kind)
            {
                case SymbolKind.Method:
                    break;
                case SymbolKind.Field:
                    break;
            }
        }

        base.VisitMemberAccessExpression(node);
    }
}