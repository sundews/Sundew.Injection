// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureInvocationMemberAccessExpressionVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base;
using Sundew.Base.Collections.Linq;

internal class ConfigureInvocationMemberAccessExpressionVisitor(
    ParameterSyntax injectionBuilderParameterSyntax,
    InvocationExpressionSyntax invocationExpressionSyntax,
    AnalysisContext analysisContext,
    CancellationToken cancellationToken)
    : CSharpSyntaxWalker
{
    private readonly ParameterSyntax injectionBuilderParameterSyntax = injectionBuilderParameterSyntax;

    public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        var symbolInfo = analysisContext.SemanticModel.GetSymbolInfo(node);
        var isGeneric = node.Name is GenericNameSyntax;
        if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
        {
            this.VisitBuilderCall(node, methodSymbol.Name, [methodSymbol], isGeneric);
            return;
        }

        if (symbolInfo.CandidateReason == CandidateReason.OverloadResolutionFailure)
        {
            var symbols = symbolInfo.CandidateSymbols.OfType<IMethodSymbol>()
                .Where(x => SymbolEqualityComparer.Default.Equals(x.ContainingType, analysisContext.KnownAnalysisTypes.InjectionBuilderType));
            if (symbols.TryGetOnlyOne(out var methodSymbol2))
            {
                this.VisitBuilderCall(node, methodSymbol2.Name, [methodSymbol2], isGeneric);
                return;
            }

            this.VisitBuilderCall(node, node.Name.Identifier.ValueText, symbolInfo.CandidateSymbols.OfType<IMethodSymbol>(), isGeneric);
        }
    }

    private void VisitBuilderCall(
        MemberAccessExpressionSyntax memberAccessExpressionSyntax,
        string name,
        IEnumerable<IMethodSymbol> methodSymbols,
        bool isGeneric)
    {
        cancellationToken.ThrowIfCancellationRequested();
        switch (name)
        {
            case nameof(IInjectionBuilder.AddParameter):
                new AddParameterVisitor(methodSymbols.First(), analysisContext).Visit(invocationExpressionSyntax);
                break;
            case nameof(IInjectionBuilder.AddParameterProperties):
                new AddParameterPropertiesVisitor(methodSymbols.First(), analysisContext).Visit(invocationExpressionSyntax);
                break;
            case nameof(IInjectionBuilder.Bind):
                if (memberAccessExpressionSyntax.Name is GenericNameSyntax bindGenericNameSyntax)
                {
                    new BindVisitor(bindGenericNameSyntax, methodSymbols.First(), analysisContext).Visit(invocationExpressionSyntax);
                }

                break;
            case nameof(IInjectionBuilder.BindGeneric):
                if (memberAccessExpressionSyntax.Name is GenericNameSyntax bindGenericGenericNameSyntax)
                {
                    new BindGenericVisitor(bindGenericGenericNameSyntax, methodSymbols.First(), analysisContext).Visit(invocationExpressionSyntax);
                }

                break;
            case nameof(IInjectionBuilder.ImplementFactory):
                if (memberAccessExpressionSyntax.Name is GenericNameSyntax implementFactoryGenericNameSyntax)
                {
                    new ImplementFactoryVisitor(implementFactoryGenericNameSyntax, methodSymbols.First(), analysisContext, invocationExpressionSyntax.GetLocation()).Visit(invocationExpressionSyntax);
                }

                break;
            case nameof(IInjectionBuilder.BindFactory):
                if (memberAccessExpressionSyntax.Name is GenericNameSyntax bindFactoryGenericNameSyntax)
                {
                    new BindFactoryVisitor(bindFactoryGenericNameSyntax, methodSymbols.First(), analysisContext).Visit(invocationExpressionSyntax);
                }

                break;
            case nameof(IInjectionBuilder.ImplementServiceProvider):
                var methodSymbol2 = methodSymbols.ByCardinality() switch
                {
                    Empty<IMethodSymbol> => null,
                    Single<IMethodSymbol> single => single.Item,
                    Multiple<IMethodSymbol> multiple => multiple.Items.FirstOrDefault(x => x.IsGenericMethod == isGeneric),
                };

                if (!methodSymbol2.HasValue())
                {
                    return;
                }

                if (memberAccessExpressionSyntax.Name is GenericNameSyntax implementServiceProviderGenericNameSyntax)
                {
                    new ImplementServiceProviderVisitor(implementServiceProviderGenericNameSyntax, methodSymbol2, analysisContext, invocationExpressionSyntax.GetLocation()).Visit(invocationExpressionSyntax);
                }

                break;
        }
    }
}