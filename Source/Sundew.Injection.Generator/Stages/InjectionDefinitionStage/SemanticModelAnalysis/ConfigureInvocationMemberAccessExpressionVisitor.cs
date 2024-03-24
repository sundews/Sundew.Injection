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

internal class ConfigureInvocationMemberAccessExpressionVisitor : CSharpSyntaxWalker
{
    private readonly ParameterSyntax injectionBuilderParameterSyntax;
    private readonly InvocationExpressionSyntax invocationExpressionSyntax;
    private readonly AnalysisContext analysisContext;
    private readonly CancellationToken cancellationToken;

    public ConfigureInvocationMemberAccessExpressionVisitor(
        ParameterSyntax injectionBuilderParameterSyntax,
        InvocationExpressionSyntax invocationExpressionSyntax,
        AnalysisContext analysisContext,
        CancellationToken cancellationToken)
    {
        this.injectionBuilderParameterSyntax = injectionBuilderParameterSyntax;
        this.invocationExpressionSyntax = invocationExpressionSyntax;
        this.analysisContext = analysisContext;
        this.cancellationToken = cancellationToken;
    }

    public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        var symbolInfo = this.analysisContext.SemanticModel.GetSymbolInfo(node);
        var isGeneric = node.Name is GenericNameSyntax;
        if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
        {
            this.VisitBuilderCall(this.invocationExpressionSyntax, methodSymbol.Name, new[] { methodSymbol }, isGeneric);
            return;
        }

        if (symbolInfo.CandidateReason == CandidateReason.OverloadResolutionFailure)
        {
            var symbols = symbolInfo.CandidateSymbols.OfType<IMethodSymbol>()
                .Where(x => SymbolEqualityComparer.Default.Equals(x.ContainingType, this.analysisContext.KnownAnalysisTypes.InjectionBuilderType));
            if (symbols.TryGetOnlyOne(out var methodSymbol2))
            {
                this.VisitBuilderCall(this.invocationExpressionSyntax, methodSymbol2.Name, new[] { methodSymbol2 }, isGeneric);
                return;
            }

            this.VisitBuilderCall(this.invocationExpressionSyntax, node.Name.Identifier.ValueText, symbolInfo.CandidateSymbols.OfType<IMethodSymbol>(), isGeneric);
        }
    }

    private void VisitBuilderCall(
        InvocationExpressionSyntax node,
        string name,
        IEnumerable<IMethodSymbol> methodSymbols,
        bool isGeneric)
    {
        this.cancellationToken.ThrowIfCancellationRequested();
        switch (name)
        {
            case nameof(IInjectionBuilder.AddParameter):
                new AddParameterVisitor(methodSymbols.First(), this.analysisContext).Visit(node);
                break;
            case nameof(IInjectionBuilder.AddParameterProperties):
                new AddParameterPropertiesVisitor(methodSymbols.First(), this.analysisContext).Visit(node);
                break;
            case nameof(IInjectionBuilder.Bind):
                new BindVisitor(methodSymbols.First(), this.analysisContext).Visit(node);
                break;
            case nameof(IInjectionBuilder.BindGeneric):
                new BindGenericVisitor(methodSymbols.First(), this.analysisContext).Visit(node);
                break;
            case nameof(IInjectionBuilder.CreateFactory):
                var methodSymbol = methodSymbols.First();
                if (methodSymbol.IsGenericMethod)
                {
                    new CreateFactoryGenericVisitor(methodSymbol, this.analysisContext).Visit(node);
                    return;
                }

                new CreateFactoryVisitor(methodSymbol, this.analysisContext).Visit(node);
                break;
            case nameof(IInjectionBuilder.BindFactory):
                new BindFactoryVisitor(methodSymbols.First(), this.analysisContext).Visit(node);
                break;
            case nameof(IInjectionBuilder.CreateResolver):
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

                if (methodSymbol2.IsGenericMethod)
                {
                    new CreateResolverGenericVisitor(methodSymbol2, this.analysisContext).Visit(node);
                    return;
                }

                new CreateResolverVisitor(methodSymbol2, this.analysisContext).Visit(node);
                break;
        }
    }
}