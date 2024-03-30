// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionDeclarationVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

internal class InjectionDeclarationVisitor : CSharpSyntaxWalker
{
    private readonly AnalysisContext analysisContext;
    private readonly CancellationToken cancellationToken;

    public InjectionDeclarationVisitor(
        AnalysisContext analysisContext,
        CancellationToken cancellationToken)
    {
        this.analysisContext = analysisContext;
        this.cancellationToken = cancellationToken;
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var classSymbol = this.analysisContext.SemanticModel.GetDeclaredSymbol(node);
        if (classSymbol != null && classSymbol.AllInterfaces.Any(@interface => SymbolEqualityComparer.Default.Equals(this.analysisContext.KnownAnalysisTypes.InjectionDeclarationType, @interface)))
        {
            base.VisitClassDeclaration(node);
        }
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var symbol = this.analysisContext.SemanticModel.GetDeclaredSymbol(node);
        if (symbol != null && symbol.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Public && Equals(symbol.Name, nameof(IInjectionDeclaration.Configure)))
        {
            var parameterSyntax = node.ParameterList.Parameters.FirstOrDefault();
            var parameterSymbol = symbol.Parameters.FirstOrDefault();
            if (parameterSyntax.HasValue() && parameterSymbol.HasValue() && parameterSymbol.DeclaringSyntaxReferences.Any(x => x.GetSyntax(this.cancellationToken) == parameterSyntax))
            {
                new ConfigureInvocationExpressionVisitor(parameterSyntax, this.analysisContext, this.cancellationToken).VisitMethodDeclaration(node);
            }
        }
    }
}