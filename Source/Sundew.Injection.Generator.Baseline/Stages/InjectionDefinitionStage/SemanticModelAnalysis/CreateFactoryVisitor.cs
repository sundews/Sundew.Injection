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
using Sundew.Base;
using Sundew.Injection.Generator.TypeSystem;

internal class CreateFactoryVisitor : CSharpSyntaxWalker
{
    private readonly SemanticModel semanticModel;
    private readonly TypeFactory typeFactory;
    private readonly KnownAnalysisTypes knownAnalysisTypes;
    private readonly CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder;
    private readonly IMethodSymbol methodSymbol;

    public CreateFactoryVisitor(SemanticModel semanticModel, TypeFactory typeFactory, KnownAnalysisTypes knownAnalysisTypes, CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder, IMethodSymbol methodSymbol)
    {
        this.semanticModel = semanticModel;
        this.typeFactory = typeFactory;
        this.knownAnalysisTypes = knownAnalysisTypes;
        this.compiletimeInjectionDefinitionBuilder = compiletimeInjectionDefinitionBuilder;
        this.methodSymbol = methodSymbol;
    }

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        var parameters = this.methodSymbol.Parameters;
        var factoryMethods = new FactoryMethodRegistrationBuilder(this.typeFactory);
        var i = 1;
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
                    case nameof(factoryMethods):
                        new FactoryMethodVisitor(this.semanticModel, this.typeFactory, this.knownAnalysisTypes.FactoryMethodSelectorTypeSymbol, this.compiletimeInjectionDefinitionBuilder, factoryMethods).Visit(argumentSyntax);
                        break;
                    case nameof(@namespace):
                        @namespace = (string?)this.semanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value;
                        break;
                    case nameof(factoryName):
                        factoryName = (string?)this.semanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value;
                        break;
                    case nameof(generateInterface):
                        generateInterface = (bool?)this.semanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value ?? true;
                        break;
                    case nameof(accessibility):
                        accessibility = this.semanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value.ToEnumOrDefault(Injection.Accessibility.Public);
                        break;
                }
            }
            else
            {
                switch (argumentIndex)
                {
                    case 0:
                        new FactoryMethodVisitor(this.semanticModel, this.typeFactory, this.knownAnalysisTypes.FactoryMethodSelectorTypeSymbol, this.compiletimeInjectionDefinitionBuilder, factoryMethods).Visit(argumentSyntax);
                        break;
                    case 1:
                        factoryName = (string?)this.semanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value;
                        break;
                    case 2:
                        generateInterface = (bool?)this.semanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value ?? true;
                        break;
                    case 3:
                        accessibility = this.semanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value.ToEnumOrDefault(Injection.Accessibility.Public);
                        break;
                    case 4:
                        @namespace = (string?)this.semanticModel.GetConstantValue((LiteralExpressionSyntax)argumentSyntax.Expression).Value;
                        break;
                }

                argumentIndex++;
            }
        }

        this.compiletimeInjectionDefinitionBuilder.CreateFactory(factoryMethods, @namespace, factoryName, generateInterface, accessibility);
    }

    public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        var symbolInfo = this.semanticModel.GetSymbolInfo(node);
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