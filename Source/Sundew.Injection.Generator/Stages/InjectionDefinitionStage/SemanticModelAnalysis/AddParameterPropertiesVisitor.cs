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
    private readonly InvocationExpressionSyntax originatingSyntax;
    private readonly AnalysisContext analysisContext;
    private readonly IMethodSymbol methodSymbol;
    private readonly Type type;
    private readonly ITypeSymbol argumentTypeSymbol;

    public AddParameterPropertiesVisitor(InvocationExpressionSyntax originatingSyntax, IMethodSymbol methodSymbol, AnalysisContext analysisContext)
    {
        this.originatingSyntax = originatingSyntax;
        this.analysisContext = analysisContext;
        this.methodSymbol = methodSymbol;
        this.type = this.analysisContext.TypeFactory.CreateType(methodSymbol.TypeArguments.First()).Type;
        this.argumentTypeSymbol = methodSymbol.TypeArguments.First();
    }

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        var parameters = this.methodSymbol.Parameters;
        var i = 0;
        var scope = (Scope: (Scope?)parameters[i++].ExplicitDefaultValue ?? Scope._SingleInstancePerRequest, Origin: ScopeOrigin.Default);
        var argumentIndex = 0;
        foreach (var argumentSyntax in node.Arguments)
        {
            if (argumentSyntax.NameColon != null)
            {
                switch (argumentSyntax.NameColon.Name.ToString())
                {
                    case nameof(scope):
                        scope = this.GetScope(argumentSyntax);
                        break;
                }
            }
            else
            {
                switch (argumentIndex)
                {
                    case 0:
                        scope = this.GetScope(argumentSyntax);
                        break;
                }

                argumentIndex++;
            }
        }

        foreach (var accessorProperty in this.argumentTypeSymbol.GetMembers().OfType<IPropertySymbol>()
                     .Where(x => !x.IsStatic && x.GetMethod != null && x.DeclaredAccessibility == Accessibility.Public).Select(x =>
                     {
                         var propertyType = this.analysisContext.TypeFactory.CreateType(x.Type);
                         var resultType = propertyType;
                         var isFunc = false;
                         if (x.Type is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType && SymbolEqualityComparer.Default.Equals(namedTypeSymbol.OriginalDefinition, this.analysisContext.KnownAnalysisTypes.FuncTypeSymbol))
                         {
                             isFunc = true;
                             resultType = this.analysisContext.TypeFactory.CreateType(namedTypeSymbol.TypeArguments.First());
                         }

                         return (isFunc, Accessor: new AccessorProperty(this.analysisContext.TypeFactory.CreateNamedType(x.ContainingType), resultType, propertyType, x.Name));
                     }))
        {
            if (accessorProperty.isFunc)
            {
                if (scope.Origin == ScopeOrigin.Default)
                {
                    scope = (Scope._NewInstance, ScopeOrigin.Implicit);
                }

                this.analysisContext.CompiletimeInjectionDefinitionBuilder.AddPropertyParameter(accessorProperty.Accessor.Result.Type, accessorProperty.Accessor, true, scope);
            }

            this.analysisContext.CompiletimeInjectionDefinitionBuilder.AddPropertyParameter(accessorProperty.Accessor.Property.Type, accessorProperty.Accessor, false, scope);
        }

        base.VisitArgumentList(node);
    }

    private (Scope Scope, ScopeOrigin Origin) GetScope(ArgumentSyntax argumentSyntax)
    {
        return ExpressionAnalysisHelper.GetScope(this.analysisContext.SemanticModel, argumentSyntax, this.analysisContext.TypeFactory);
    }
}