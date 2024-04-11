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

internal class AddParameterPropertiesVisitor(
    IMethodSymbol methodSymbol,
    AnalysisContext analysisContext)
    : CSharpSyntaxWalker
{
    private readonly ITypeSymbol argumentTypeSymbol = methodSymbol.TypeArguments.First();

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        var parameters = methodSymbol.Parameters;
        var parameterType = analysisContext.TypeFactory.CreateType(this.argumentTypeSymbol);
        var i = 0;
        var scope = new ScopeContext((Scope?)parameters[i++].ExplicitDefaultValue ?? Scope._SingleInstancePerRequest(Location.None), ScopeSelection.Default);
        var argumentIndex = 0;
        foreach (var argumentSyntax in node.Arguments)
        {
            if (argumentSyntax.NameColon != null)
            {
                switch (argumentSyntax.NameColon.Name.ToString())
                {
                    case nameof(scope):
                        scope = this.GetScope(argumentSyntax, parameterType.Type);
                        break;
                }
            }
            else
            {
                switch (argumentIndex)
                {
                    case 0:
                        scope = this.GetScope(argumentSyntax, parameterType.Type);
                        break;
                }

                argumentIndex++;
            }
        }

        foreach (var accessorProperty in this.argumentTypeSymbol.GetMembers().OfType<IPropertySymbol>()
                     .Where(x => !x.IsStatic && x.GetMethod != null && x.DeclaredAccessibility == Accessibility.Public).Select(x =>
                     {
                         var propertyType = analysisContext.TypeFactory.CreateType(x.Type);
                         var resultType = propertyType;
                         var isFunc = false;
                         if (x.Type is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType && SymbolEqualityComparer.Default.Equals(namedTypeSymbol.OriginalDefinition, analysisContext.KnownAnalysisTypes.FuncTypeSymbol))
                         {
                             isFunc = true;
                             resultType = analysisContext.TypeFactory.CreateType(namedTypeSymbol.TypeArguments.First());
                         }

                         return (isFunc, Accessor: new AccessorProperty(analysisContext.TypeFactory.CreateNamedType(x.ContainingType), resultType, propertyType, x.Name));
                     }))
        {
            if (accessorProperty.isFunc)
            {
                if (scope.Selection == ScopeSelection.Default)
                {
                    scope = new ScopeContext(Scope._NewInstance(Location.None), ScopeSelection.Implicit);
                }

                analysisContext.CompiletimeInjectionDefinitionBuilder.AddPropertyParameter(accessorProperty.Accessor.Result.Type, accessorProperty.Accessor, true, scope);
            }

            analysisContext.CompiletimeInjectionDefinitionBuilder.AddPropertyParameter(accessorProperty.Accessor.Property.Type, accessorProperty.Accessor, false, scope);
        }

        base.VisitArgumentList(node);
    }

    private ScopeContext GetScope(ArgumentSyntax argumentSyntax, Symbol targetType)
    {
        return ExpressionAnalysisHelper.GetScope(analysisContext.SemanticModel, argumentSyntax, analysisContext.TypeFactory, targetType);
    }
}