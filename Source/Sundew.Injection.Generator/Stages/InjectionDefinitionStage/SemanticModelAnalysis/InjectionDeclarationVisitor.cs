// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionDeclarationVisitor.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Injection.Generator.TypeSystem;

internal class InjectionDeclarationVisitor : CSharpSyntaxWalker
{
    private readonly SemanticModel semanticModel;
    private readonly KnownAnalysisTypes knownAnalysisTypes;
    private readonly CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder;
    private readonly TypeFactory typeFactory;
    private readonly CancellationToken cancellationToken;

    public InjectionDeclarationVisitor(
        SemanticModel semanticModel,
        KnownAnalysisTypes knownAnalysisTypes,
        CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder,
        TypeFactory typeFactory,
        CancellationToken cancellationToken)
    {
        this.semanticModel = semanticModel;
        this.knownAnalysisTypes = knownAnalysisTypes;
        this.compiletimeInjectionDefinitionBuilder = compiletimeInjectionDefinitionBuilder;
        this.typeFactory = typeFactory;
        this.cancellationToken = cancellationToken;
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var classSymbol = this.semanticModel.GetDeclaredSymbol(node);
        if (classSymbol != null && classSymbol.AllInterfaces.Any(@interface => SymbolEqualityComparer.Default.Equals(this.knownAnalysisTypes.InjectionDeclarationType, @interface)))
        {
            this.compiletimeInjectionDefinitionBuilder.DefaultNamespace = classSymbol.ContainingNamespace.ToDisplayString();
            base.VisitClassDeclaration(node);
        }
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var symbol = this.semanticModel.GetDeclaredSymbol(node);
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
                var symbolInfo = this.semanticModel.GetSymbolInfo(memberAccessExpressionSyntax.Expression);
                if (symbolInfo.Symbol is IParameterSymbol parameterSymbol && SymbolEqualityComparer.Default.Equals(parameterSymbol.Type, this.knownAnalysisTypes.InjectionBuilderType) && memberAccessExpressionSyntax.Name.Identifier.Text == nameof(IInjectionBuilder.RequiredParameterInjection))
                {
                    new RequiredParameterInjectionVisitor(this.compiletimeInjectionDefinitionBuilder).Visit(node.Right);
                }

                break;
        }

        base.VisitAssignmentExpression(node);
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        var symbolInfo = this.semanticModel.GetSymbolInfo(node);
        if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
        {
            this.VisitBuilderCall(node, methodSymbol);
        }
        else if (symbolInfo.CandidateReason == CandidateReason.OverloadResolutionFailure && symbolInfo.CandidateSymbols.Length == 1)
        {
            if (symbolInfo.CandidateSymbols[0] is IMethodSymbol methodSymbol2 && SymbolEqualityComparer.Default.Equals(methodSymbol2.ContainingType, this.knownAnalysisTypes.InjectionBuilderType))
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
                new AddParameterVisitor(this.semanticModel, this.typeFactory, this.compiletimeInjectionDefinitionBuilder, methodSymbol).Visit(node);
                break;
            case nameof(Injection.IInjectionBuilder.AddParameterProperties):
                new AddParameterPropertiesVisitor(this.semanticModel, this.typeFactory, this.compiletimeInjectionDefinitionBuilder, this.knownAnalysisTypes, methodSymbol).Visit(node);
                break;
            case nameof(Injection.IInjectionBuilder.Bind):
                new BindVisitor(this.semanticModel, this.typeFactory, this.compiletimeInjectionDefinitionBuilder, methodSymbol).Visit(node);
                break;
            case nameof(Injection.IInjectionBuilder.BindGeneric):
                new BindGenericVisitor(this.semanticModel, this.typeFactory, this.compiletimeInjectionDefinitionBuilder, methodSymbol).Visit(node);
                break;
            case nameof(Injection.IInjectionBuilder.CreateFactory):
                if (methodSymbol.Parameters.Length == 4)
                {
                    new CreateFactoryGenericVisitor(this.semanticModel, this.typeFactory, this.knownAnalysisTypes, this.compiletimeInjectionDefinitionBuilder, methodSymbol).Visit(node);
                    return;
                }

                new CreateFactoryVisitor(this.semanticModel, this.typeFactory, this.knownAnalysisTypes, this.compiletimeInjectionDefinitionBuilder, methodSymbol).Visit(node);
                break;
        }
    }
}