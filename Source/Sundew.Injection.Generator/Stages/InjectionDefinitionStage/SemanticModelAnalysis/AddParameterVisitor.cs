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
        this.type = this.analysisContext.TypeFactory.CreateType(methodSymbol.TypeArguments.First()).Type;
    }

    public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        var symbolInfo = this.analysisContext.SemanticModel.GetSymbolInfo(node);
        if (symbolInfo.Symbol != null)
        {
            switch (symbolInfo.Symbol.Kind)
            {
                case SymbolKind.Method:
                    break;
                case SymbolKind.Field:
                    var inject = symbolInfo.Symbol.Name.ParseEnum<Inject>();
                    this.analysisContext.CompiletimeInjectionDefinitionBuilder.AddParameter(this.type, inject);
                    break;
            }
        }

        base.VisitMemberAccessExpression(node);
    }
}