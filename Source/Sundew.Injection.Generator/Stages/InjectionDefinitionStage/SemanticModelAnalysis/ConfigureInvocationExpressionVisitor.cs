// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureInvocationExpressionVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base.Collections.Linq;

internal class ConfigureInvocationExpressionVisitor : CSharpSyntaxWalker
{
    private readonly ParameterSyntax injectionBuilderParameterSyntax;
    private readonly AnalysisContext analysisContext;
    private readonly CancellationToken cancellationToken;

    public ConfigureInvocationExpressionVisitor(ParameterSyntax injectionBuilderParameterSyntax, AnalysisContext analysisContext, CancellationToken cancellationToken)
    {
        this.injectionBuilderParameterSyntax = injectionBuilderParameterSyntax;
        this.analysisContext = analysisContext;
        this.cancellationToken = cancellationToken;
    }

    public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
    {
        switch (node.Left)
        {
            case MemberAccessExpressionSyntax memberAccessExpressionSyntax:
                var symbolInfo = this.analysisContext.SemanticModel.GetSymbolInfo(memberAccessExpressionSyntax.Expression);
                if (symbolInfo.Symbol is IParameterSymbol parameterSymbol && SymbolEqualityComparer.Default.Equals(parameterSymbol.Type, this.analysisContext.KnownAnalysisTypes.InjectionBuilderType) && memberAccessExpressionSyntax.Name.Identifier.Text == nameof(IInjectionBuilder.RequiredParameterInjection))
                {
                    new RequiredParameterInjectionVisitor(this.analysisContext.CompiletimeInjectionDefinitionBuilder).Visit(node.Right);
                }

                break;
        }

        base.VisitAssignmentExpression(node);
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        new ConfigureInvocationMemberAccessExpressionVisitor(this.injectionBuilderParameterSyntax, node, this.analysisContext, this.cancellationToken).VisitInvocationExpression(node);
    }
}