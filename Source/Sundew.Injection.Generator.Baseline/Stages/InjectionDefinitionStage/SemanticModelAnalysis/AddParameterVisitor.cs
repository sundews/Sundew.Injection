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
    private readonly SemanticModel semanticModel;
    private readonly TypeFactory typeFactory;
    private readonly CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder;
    private readonly IMethodSymbol symbol;
    private readonly Type type;

    public AddParameterVisitor(SemanticModel semanticModel, TypeFactory typeFactory, CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder, IMethodSymbol symbol)
    {
        this.semanticModel = semanticModel;
        this.typeFactory = typeFactory;
        this.compiletimeInjectionDefinitionBuilder = compiletimeInjectionDefinitionBuilder;
        this.symbol = symbol;
        this.type = this.typeFactory.CreateType(symbol.TypeArguments.First()).Type;
    }

    public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        var symbolInfo = this.semanticModel.GetSymbolInfo(node);
        if (symbolInfo.Symbol != null)
        {
            switch (symbolInfo.Symbol.Kind)
            {
                case SymbolKind.Method:
                    break;
                case SymbolKind.Field:
                    var inject = symbolInfo.Symbol.Name.ParseEnum<Inject>();
                    this.compiletimeInjectionDefinitionBuilder.AddParameter(this.type, inject);
                    break;
            }
        }

        base.VisitMemberAccessExpression(node);
    }
}