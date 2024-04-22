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
using Sundew.Injection.Generator.TypeSystem;

internal class BindMethodVisitor(
    TypeSymbolWithLocation factoryTypeSymbolWithLocation,
    AnalysisContext analysisContext)
    : CSharpSyntaxWalker
{
    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        base.VisitInvocationExpression(node);
        var symbolInfo = analysisContext.SemanticModel.GetSymbolInfo(node);
        if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
        {
            this.VisitBuilderCall(node, methodSymbol);
        }
        else if (symbolInfo is { CandidateReason: CandidateReason.OverloadResolutionFailure, CandidateSymbols: [IMethodSymbol singleCandidateMethodSymbol] })
        {
            this.VisitBuilderCall(node, singleCandidateMethodSymbol);
        }
    }

    private void VisitBuilderCall(InvocationExpressionSyntax node, IMethodSymbol methodSymbol)
    {
        var factoryTypeResult = analysisContext.TypeFactory.GetFullType(factoryTypeSymbolWithLocation.TypeSymbol);
        if (!factoryTypeResult.TryGet(out var factoryType, out var errors))
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.InfiniteRecursionError, factoryTypeSymbolWithLocation, errors.GetErrorText());
            return;
        }

        if (methodSymbol.Name == nameof(IFactoryMethodBindingSelector<object>.Add) &&
            SymbolEqualityComparer.Default.Equals(methodSymbol.ContainingType, analysisContext.KnownAnalysisTypes.FactoryMethodBindingSelectorTypeSymbol))
        {
            var addFactoryMethodBindingVisitor = new AddFactoryMethodBindingVisitor(methodSymbol, analysisContext);
            addFactoryMethodBindingVisitor.Visit(node);
            analysisContext.BindFactory(factoryType, addFactoryMethodBindingVisitor.FactoryMethods);
        }
        else if (SymbolEqualityComparer.Default.Equals(methodSymbol.ContainingType, factoryTypeSymbolWithLocation.TypeSymbol))
        {
            var factoryMethodResult = analysisContext.TypeFactory.GetFactoryMethod(methodSymbol);
            if (factoryMethodResult.IsError)
            {
                analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.InfiniteRecursionError, factoryTypeSymbolWithLocation, (factoryMethodResult.Error with { Symbol = new NamedSymbol(methodSymbol.ToDisplayString()) }).GetErrorText());
                return;
            }

            var createMethods = ImmutableArray.Create((Method: factoryMethodResult.Value, ReturnType: factoryTypeSymbolWithLocation with { TypeSymbol = methodSymbol.ReturnType }));
            analysisContext.BindFactory(factoryType, createMethods);
        }
    }
}