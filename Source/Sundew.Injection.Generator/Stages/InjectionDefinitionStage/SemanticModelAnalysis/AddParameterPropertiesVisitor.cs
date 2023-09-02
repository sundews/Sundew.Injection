// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddParameterPropertiesVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal class AddParameterPropertiesVisitor : CSharpSyntaxWalker
{
    private readonly SemanticModel semanticModel;
    private readonly TypeFactory typeFactory;
    private readonly CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder;
    private readonly KnownAnalysisTypes knownAnalysisTypes;
    private readonly IMethodSymbol symbol;
    private readonly Type type;
    private readonly ITypeSymbol argumentTypeSymbol;

    public AddParameterPropertiesVisitor(SemanticModel semanticModel, TypeFactory typeFactory, CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder, KnownAnalysisTypes knownAnalysisTypes, IMethodSymbol symbol)
    {
        this.semanticModel = semanticModel;
        this.typeFactory = typeFactory;
        this.compiletimeInjectionDefinitionBuilder = compiletimeInjectionDefinitionBuilder;
        this.knownAnalysisTypes = knownAnalysisTypes;
        this.symbol = symbol;
        this.type = this.typeFactory.CreateType(symbol.TypeArguments.First()).Type;
        this.argumentTypeSymbol = symbol.TypeArguments.First();
    }

    public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        var symbolInfo = this.semanticModel.GetSymbolInfo(node);
        if (symbolInfo.Symbol != null)
        {
            switch (symbolInfo.Symbol.Kind)
            {
                case SymbolKind.Method:
                    foreach (var accessorProperty in this.argumentTypeSymbol.GetMembers().OfType<IPropertySymbol>()
                        .Where(x => !x.IsStatic && x.GetMethod != null && x.DeclaredAccessibility == Accessibility.Public).Select(x =>
                        {
                            var propertyType = this.typeFactory.CreateType(x.Type);
                            var resultType = propertyType;
                            var registerBoth = false;
                            if (x.Type is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType && SymbolEqualityComparer.Default.Equals(namedTypeSymbol.OriginalDefinition, this.knownAnalysisTypes.FuncTypeSymbol))
                            {
                                registerBoth = true;
                                resultType = this.typeFactory.CreateType(namedTypeSymbol.TypeArguments.First());
                            }

                            return (registerBoth, Accessor: new AccessorProperty(this.typeFactory.CreateNamedType(x.ContainingType), resultType, propertyType, x.Name));
                        }))
                    {
                        this.compiletimeInjectionDefinitionBuilder.AddPropertyParameter(accessorProperty.Accessor.Property.Type, accessorProperty.Accessor);
                        if (accessorProperty.registerBoth)
                        {
                            this.compiletimeInjectionDefinitionBuilder.AddPropertyParameter(accessorProperty.Accessor.Result.Type, accessorProperty.Accessor);
                        }
                    }

                    break;
            }
        }

        base.VisitMemberAccessExpression(node);
    }
}