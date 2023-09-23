// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateFactoryVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base.Primitives;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

internal class CreateFactoryVisitor : CSharpSyntaxWalker
{
    private readonly IMethodSymbol methodSymbol;
    private readonly AnalysisContext analysisContext;

    public CreateFactoryVisitor(IMethodSymbol methodSymbol, AnalysisContext analysisContext)
    {
        this.methodSymbol = methodSymbol;
        this.analysisContext = analysisContext;
    }

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        var parameters = this.methodSymbol.Parameters;
        var i = 1;
        var factoryMethods = new FactoryMethodRegistrationBuilder();
        var factoryName = (string?)parameters[i++].ExplicitDefaultValue;
        var generateInterface = (bool?)parameters[i++].ExplicitDefaultValue ?? true;
        var accessibility = parameters[i++].ExplicitDefaultValue.ToEnumOrDefault(Injection.Accessibility.Public);
        var @namespace = (string?)parameters[i++].ExplicitDefaultValue;
        var generateTypeResolver = (bool?)parameters[i++].ExplicitDefaultValue ?? false;
        var argumentIndex = 0;
        foreach (var argumentSyntax in node.Arguments)
        {
            if (argumentSyntax.NameColon != null)
            {
                switch (argumentSyntax.NameColon.Name.ToString())
                {
                    case nameof(factoryMethods):
                        new FactoryMethodVisitor(factoryMethods, this.analysisContext).Visit(argumentSyntax);
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
                    case nameof(@namespace):
                        @namespace = (string?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value;
                        break;
                    case nameof(generateTypeResolver):
                        generateTypeResolver = (bool?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value ?? false;
                        break;
                }
            }
            else
            {
                switch (argumentIndex)
                {
                    case 0:
                        new FactoryMethodVisitor(factoryMethods, this.analysisContext).Visit(argumentSyntax);
                        break;
                    case 1:
                        factoryName = (string?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value;
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
                    case 5:
                        generateTypeResolver = (bool?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value ?? false;
                        break;
                }

                argumentIndex++;
            }
        }

        this.analysisContext.CompiletimeInjectionDefinitionBuilder.CreateFactory(factoryMethods, @namespace, factoryName, generateInterface, accessibility, generateTypeResolver);
    }
}