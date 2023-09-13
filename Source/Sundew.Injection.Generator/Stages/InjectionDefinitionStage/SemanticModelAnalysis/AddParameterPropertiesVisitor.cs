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
    private readonly AnalysisContext analysisContext;
    private readonly IMethodSymbol methodSymbol;
    private readonly Type type;
    private readonly ITypeSymbol argumentTypeSymbol;

    public AddParameterPropertiesVisitor(IMethodSymbol methodSymbol, AnalysisContext analysisContext)
    {
        this.analysisContext = analysisContext;
        this.methodSymbol = methodSymbol;
        this.type = this.analysisContext.TypeFactory.CreateType(methodSymbol.TypeArguments.First()).Type;
        this.argumentTypeSymbol = methodSymbol.TypeArguments.First();
    }

    public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        var symbolInfo = this.analysisContext.SemanticModel.GetSymbolInfo(node);
        if (symbolInfo.Symbol != null)
        {
            switch (symbolInfo.Symbol.Kind)
            {
                case SymbolKind.Method:
                    foreach (var accessorProperty in this.argumentTypeSymbol.GetMembers().OfType<IPropertySymbol>()
                        .Where(x => !x.IsStatic && x.GetMethod != null && x.DeclaredAccessibility == Accessibility.Public).Select(x =>
                        {
                            var propertyType = this.analysisContext.TypeFactory.CreateType(x.Type);
                            var resultType = propertyType;
                            var registerBoth = false;
                            if (x.Type is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType && SymbolEqualityComparer.Default.Equals(namedTypeSymbol.OriginalDefinition, this.analysisContext.KnownAnalysisTypes.FuncTypeSymbol))
                            {
                                registerBoth = true;
                                resultType = this.analysisContext.TypeFactory.CreateType(namedTypeSymbol.TypeArguments.First());
                            }

                            return (registerBoth, Accessor: new AccessorProperty(this.analysisContext.TypeFactory.CreateNamedType(x.ContainingType), resultType, propertyType, x.Name));
                        }))
                    {
                        this.analysisContext.CompiletimeInjectionDefinitionBuilder.AddPropertyParameter(accessorProperty.Accessor.Property.Type, accessorProperty.Accessor);
                        if (accessorProperty.registerBoth)
                        {
                            this.analysisContext.CompiletimeInjectionDefinitionBuilder.AddPropertyParameter(accessorProperty.Accessor.Result.Type, accessorProperty.Accessor);
                        }
                    }

                    break;
            }
        }

        base.VisitMemberAccessExpression(node);
    }
}