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
            this.analysisContext.CompiletimeInjectionDefinitionBuilder.DefaultNamespace = classSymbol.ContainingNamespace.ToDisplayString();
            base.VisitClassDeclaration(node);
        }
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var symbol = this.analysisContext.SemanticModel.GetDeclaredSymbol(node);
        if (symbol != null && symbol.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Public && Equals(symbol.Name, nameof(IInjectionDeclaration.Configure)))
        {
            base.VisitMethodDeclaration(node);
        }
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
        var symbolInfo = this.analysisContext.SemanticModel.GetSymbolInfo(node);
        if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
        {
            this.VisitBuilderCall(node, methodSymbol);
        }
        else if (symbolInfo.CandidateReason == CandidateReason.OverloadResolutionFailure && symbolInfo.CandidateSymbols.Length == 1)
        {
            if (symbolInfo.CandidateSymbols[0] is IMethodSymbol methodSymbol2 && SymbolEqualityComparer.Default.Equals(methodSymbol2.ContainingType, this.analysisContext.KnownAnalysisTypes.InjectionBuilderType))
            {
                this.VisitBuilderCall(node, methodSymbol2);
            }
        }

        base.VisitInvocationExpression(node);
    }

    private void VisitBuilderCall(InvocationExpressionSyntax node, IMethodSymbol methodSymbol)
    {
        this.cancellationToken.ThrowIfCancellationRequested();
        switch (methodSymbol.Name)
        {
            case nameof(Injection.IInjectionBuilder.AddParameter):
                new AddParameterVisitor(methodSymbol, this.analysisContext).Visit(node);
                break;
            case nameof(Injection.IInjectionBuilder.AddParameterProperties):
                new AddParameterPropertiesVisitor(methodSymbol, this.analysisContext).Visit(node);
                break;
            case nameof(Injection.IInjectionBuilder.Bind):
                new BindVisitor(methodSymbol, this.analysisContext).Visit(node);
                break;
            case nameof(Injection.IInjectionBuilder.BindGeneric):
                new BindGenericVisitor(methodSymbol, this.analysisContext).Visit(node);
                break;
            case nameof(Injection.IInjectionBuilder.CreateFactory):
                if (methodSymbol.IsGenericMethod)
                {
                    new CreateFactoryGenericVisitor(methodSymbol, this.analysisContext).Visit(node);
                    return;
                }

                new CreateFactoryVisitor(methodSymbol, this.analysisContext).Visit(node);
                break;
            case nameof(IInjectionBuilder.BindFactory):
                new BindFactoryVisitor(methodSymbol, this.analysisContext).Visit(node);
                break;
        }
    }
}