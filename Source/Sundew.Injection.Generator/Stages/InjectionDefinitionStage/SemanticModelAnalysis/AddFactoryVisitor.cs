// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddFactoryVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal class AddFactoryVisitor(
    GenericNameSyntax addGenericNameSyntax,
    IMethodSymbol addMethodSymbol,
    FactoryRegistrationBuilder factoryRegistrationBuilder,
    AnalysisContext analysisContext)
    : CSharpSyntaxWalker
{
    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        this.VisitArgumentList(node.ArgumentList);
    }

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        base.VisitArgumentList(node);
        analysisContext.AddFactoryFromTypeSymbol(addMethodSymbol.MapTypeArguments(addGenericNameSyntax).First(), factoryRegistrationBuilder);
    }
}