// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddFactoryVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal class AddFactoryVisitor : CSharpSyntaxWalker
{
    private readonly FactoryRegistrationBuilder factoryRegistrationBuilder;
    private readonly IMethodSymbol methodSymbol;
    private readonly AnalysisContext analysisContext;

    public AddFactoryVisitor(IMethodSymbol methodSymbol, FactoryRegistrationBuilder factoryRegistrationBuilder, AnalysisContext analysisContext)
    {
        this.methodSymbol = methodSymbol;
        this.factoryRegistrationBuilder = factoryRegistrationBuilder;
        this.analysisContext = analysisContext;
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        this.VisitArgumentList(node.ArgumentList);
    }

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        base.VisitArgumentList(node);
        this.analysisContext.AddFactoryFromTypeSymbol(this.methodSymbol.TypeArguments[0], this.factoryRegistrationBuilder);
    }
}