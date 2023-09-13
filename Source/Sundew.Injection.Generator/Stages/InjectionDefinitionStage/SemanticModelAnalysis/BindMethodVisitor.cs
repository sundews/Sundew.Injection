// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethodVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal class BindMethodVisitor : CSharpSyntaxWalker
{
    private readonly ITypeSymbol factoryTypeSymbol;
    private readonly AnalysisContext analysisContext;

    public BindMethodVisitor(ITypeSymbol factoryTypeSymbol, AnalysisContext analysisContext)
    {
        this.factoryTypeSymbol = factoryTypeSymbol;
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
            if (symbolInfo.CandidateSymbols[0] is IMethodSymbol methodSymbol2)
            {
                this.VisitBuilderCall(node, methodSymbol2);
            }
        }
    }

    private void VisitBuilderCall(InvocationExpressionSyntax node, IMethodSymbol methodSymbol)
    {
        var factoryType = this.analysisContext.TypeFactory.CreateType(this.factoryTypeSymbol);
        if (methodSymbol.Name == nameof(ICreateMethodSelector<object>.Add) && SymbolEqualityComparer.Default.Equals(methodSymbol.ContainingType, this.analysisContext.KnownAnalysisTypes.CreateMethodSelectorTypeSymbol))
        {
            var addCreateMethodVisitor = new AddCreateMethodVisitor(methodSymbol, this.analysisContext);
            addCreateMethodVisitor.Visit(node);
            this.analysisContext.BindFactory(factoryType, addCreateMethodVisitor.CreateMethods);
        }
        else if (SymbolEqualityComparer.Default.Equals(methodSymbol.ContainingType, this.factoryTypeSymbol))
        {
            var createMethods = ImmutableArray.Create((Method: this.analysisContext.TypeFactory.CreateMethod(methodSymbol), ReturnType: methodSymbol.ReturnType));
            this.analysisContext.BindFactory(factoryType, createMethods);
        }
    }
}