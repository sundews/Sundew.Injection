// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddFactoryMethodVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base.Primitives;
using Sundew.Injection.Generator.TypeSystem;

internal class AddFactoryMethodVisitor : CSharpSyntaxWalker
{
    private readonly FactoryMethodRegistrationBuilder factoryMethodRegistrationBuilder;
    private readonly IMethodSymbol methodSymbol;
    private readonly AnalysisContext analysisContext;

    public AddFactoryMethodVisitor(IMethodSymbol methodSymbol, FactoryMethodRegistrationBuilder factoryMethodRegistrationBuilder, AnalysisContext analysisContext)
    {
        this.methodSymbol = methodSymbol;
        this.factoryMethodRegistrationBuilder = factoryMethodRegistrationBuilder;
        this.analysisContext = analysisContext;
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        this.VisitArgumentList(node.ArgumentList);
    }

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        base.VisitArgumentList(node);
        var typeArguments = this.methodSymbol.TypeArguments;
        var interfaceType = typeArguments[0];
        var implementationType = typeArguments.Length == 2 ? typeArguments[1] : interfaceType;
        var parameters = this.methodSymbol.Parameters;
        var i = 0;
        var constructorSelector = (Method?)parameters[i++].ExplicitDefaultValue;
        var createMethodName = (string?)parameters[i++].ExplicitDefaultValue;
        var accessibility = parameters[i++].ExplicitDefaultValue.ToEnumOrDefault(Injection.Accessibility.Public);
        var isNewOverridable = (bool?)parameters[i++].ExplicitDefaultValue ?? true;
        var argumentIndex = 0;
        foreach (var argumentSyntax in node.Arguments)
        {
            if (argumentSyntax.NameColon != null)
            {
                switch (argumentSyntax.NameColon.Name.ToString())
                {
                    case nameof(constructorSelector):
                        constructorSelector = this.GetMethod(argumentSyntax);
                        break;
                    case nameof(createMethodName):
                        createMethodName = (string?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value;
                        break;
                    case nameof(accessibility):
                        accessibility = this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value.ToEnumOrDefault(Injection.Accessibility.Public);
                        break;
                    case nameof(isNewOverridable):
                        isNewOverridable = (bool?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value ?? true;
                        break;
                }
            }
            else
            {
                switch (argumentIndex)
                {
                    case 0:
                        constructorSelector = this.GetMethod(argumentSyntax);
                        break;
                    case 1:
                        createMethodName = (string?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value;
                        break;
                    case 2:
                        accessibility = this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value.ToEnumOrDefault(Injection.Accessibility.Public);
                        break;
                    case 3:
                        isNewOverridable = (bool?)this.analysisContext.SemanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value ?? true;
                        break;
                }

                argumentIndex++;
            }
        }

        this.factoryMethodRegistrationBuilder.Add(interfaceType, implementationType, constructorSelector, createMethodName, accessibility, isNewOverridable);
    }

    private Method? GetMethod(ArgumentSyntax argumentSyntax)
    {
        return ExpressionAnalysisHelper.GetMethod(argumentSyntax, this.analysisContext.SemanticModel, this.analysisContext.TypeFactory);
    }
}