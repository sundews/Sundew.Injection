// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethodVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Injection.Generator.TypeSystem;

internal class FactoryMethodVisitor : CSharpSyntaxWalker
{
    private readonly SemanticModel semanticModel;
    private readonly TypeFactory typeFactory;
    private readonly CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder;
    private readonly FactoryMethodRegistrationBuilder factoryMethodRegistrationBuilder;
    private readonly INamedTypeSymbol factoryMethodSelectorTypeSymbol;

    public FactoryMethodVisitor(SemanticModel semanticModel, TypeFactory typeFactory, INamedTypeSymbol factoryMethodSelectorTypeSymbol, CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder, FactoryMethodRegistrationBuilder factoryMethodRegistrationBuilder)
    {
        this.semanticModel = semanticModel;
        this.typeFactory = typeFactory;
        this.compiletimeInjectionDefinitionBuilder = compiletimeInjectionDefinitionBuilder;
        this.factoryMethodRegistrationBuilder = factoryMethodRegistrationBuilder;
        this.factoryMethodSelectorTypeSymbol = factoryMethodSelectorTypeSymbol;
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        base.VisitInvocationExpression(node);
        var symbolInfo = this.semanticModel.GetSymbolInfo(node);
        if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
        {
            this.VisitBuilderCall(node, methodSymbol);
        }
        else if (symbolInfo.CandidateReason == CandidateReason.OverloadResolutionFailure &&
                 symbolInfo.CandidateSymbols.Length == 1)
        {
            if (symbolInfo.CandidateSymbols[0] is IMethodSymbol methodSymbol2 && SymbolEqualityComparer.Default.Equals(methodSymbol2.ContainingType, this.factoryMethodSelectorTypeSymbol))
            {
                this.VisitBuilderCall(node, methodSymbol2);
            }
        }
    }

    private void VisitBuilderCall(InvocationExpressionSyntax node, IMethodSymbol methodSymbol)
    {
        switch (methodSymbol.Name)
        {
            case nameof(IFactoryMethodSelector.Add):
                new AddFactoryMethodVisitor(this.semanticModel, this.typeFactory, this.factoryMethodRegistrationBuilder, methodSymbol).Visit(node);
                break;
        }
    }
}