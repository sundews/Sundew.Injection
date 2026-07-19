// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureInvocationExpressionVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal class ConfigureInvocationExpressionVisitor(
    ParameterSyntax injectionBuilderParameterSyntax,
    AnalysisContext analysisContext,
    CancellationToken cancellationToken)
    : CSharpSyntaxWalker
{
    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        new ConfigureInvocationMemberAccessExpressionVisitor(injectionBuilderParameterSyntax, node, analysisContext, cancellationToken).VisitInvocationExpression(node);
    }
}