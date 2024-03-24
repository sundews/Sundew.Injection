// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal class FactoryVisitor : CSharpSyntaxWalker
{
    private readonly FactoryRegistrationBuilder factoryRegistrationBuilder;
    private readonly AnalysisContext analysisContext;

    public FactoryVisitor(FactoryRegistrationBuilder factoryRegistrationBuilder, AnalysisContext analysisContext)
    {
        this.factoryRegistrationBuilder = factoryRegistrationBuilder;
        this.analysisContext = analysisContext;
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        base.VisitInvocationExpression(node);
        var symbolInfo = this.analysisContext.SemanticModel.GetSymbolInfo(node);
        if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
        {
            this.VisitBuilderCall(node, methodSymbol);
        }
        else if (symbolInfo.CandidateReason == CandidateReason.OverloadResolutionFailure &&
                 symbolInfo.CandidateSymbols.Length == 1)
        {
            if (symbolInfo.CandidateSymbols[0] is IMethodSymbol methodSymbol2 && SymbolEqualityComparer.Default.Equals(methodSymbol2.ContainingType, this.analysisContext.KnownAnalysisTypes.FactoryMethodSelectorTypeSymbol))
            {
                this.VisitBuilderCall(node, methodSymbol2);
            }
        }
    }

    private void VisitBuilderCall(InvocationExpressionSyntax node, IMethodSymbol methodSymbol)
    {
        switch (methodSymbol.Name)
        {
            case nameof(IFactorySelector.Add):
                new AddFactoryVisitor(methodSymbol, this.factoryRegistrationBuilder, this.analysisContext).Visit(node);
                break;
        }
    }
}