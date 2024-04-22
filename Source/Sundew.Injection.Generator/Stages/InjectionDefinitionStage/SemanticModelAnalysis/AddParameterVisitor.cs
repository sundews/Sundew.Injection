// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddParameterVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base;
using Sundew.Injection.Generator.TypeSystem;

internal class AddParameterVisitor : CSharpSyntaxWalker
{
    private readonly IMethodSymbol methodSymbol;
    private readonly AnalysisContext analysisContext;
    private readonly Type type;

    public AddParameterVisitor(IMethodSymbol methodSymbol, AnalysisContext analysisContext)
    {
        this.methodSymbol = methodSymbol;
        this.analysisContext = analysisContext;
        this.type = this.analysisContext.TypeFactory.GetType(methodSymbol.TypeArguments.First());
    }

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        var parameters = this.methodSymbol.Parameters;
        var i = 0;
        var inject = (Inject?)(int?)parameters[i++].ExplicitDefaultValue ?? Inject.Shared;
        var scope = new ScopeContext((Scope?)parameters[i++].ExplicitDefaultValue ?? Scope._SingleInstancePerRequest(Location.None), ScopeSelection.Implicit);
        var argumentIndex = 0;
        foreach (var argumentSyntax in node.Arguments)
        {
            if (argumentSyntax.NameColon != null)
            {
                switch (argumentSyntax.NameColon.Name.ToString())
                {
                    case nameof(inject):
                        inject = this.GetInject(argumentSyntax);
                        break;
                    case nameof(scope):
                        scope = this.GetScope(argumentSyntax, this.type);
                        break;
                }
            }
            else
            {
                switch (argumentIndex)
                {
                    case 0:
                        inject = this.GetInject(argumentSyntax);
                        break;
                    case 1:
                        scope = this.GetScope(argumentSyntax, this.type);
                        break;
                }

                argumentIndex++;
            }
        }

        this.analysisContext.CompiletimeInjectionDefinitionBuilder.AddParameter(this.type, inject, scope);
    }

    private ScopeContext GetScope(ArgumentSyntax argumentSyntax, Symbol targetType)
    {
        return ExpressionAnalysisHelper.GetScope(this.analysisContext.SemanticModel, argumentSyntax, this.analysisContext.TypeFactory, targetType);
    }

    private Inject GetInject(ArgumentSyntax argumentSyntax)
    {
        if (argumentSyntax is { Expression: MemberAccessExpressionSyntax memberAccessExpressionSyntax })
        {
            var symbolInfo = this.analysisContext.SemanticModel.GetSymbolInfo(memberAccessExpressionSyntax);
            if (symbolInfo.Symbol != null)
            {
                return symbolInfo.Symbol.Name.ParseEnum<Inject>();
            }
        }

        return Inject.Shared;
    }
}