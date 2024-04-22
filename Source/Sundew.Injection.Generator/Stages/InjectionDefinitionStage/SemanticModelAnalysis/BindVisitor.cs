// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base;
using Sundew.Injection.Generator.TypeSystem;

internal class BindVisitor(
    GenericNameSyntax bindGenericNameSyntax,
    IMethodSymbol methodSymbol,
    AnalysisContext analysisContext)
    : CSharpSyntaxWalker
{
    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        var typeArguments = methodSymbol.MapTypeArguments(bindGenericNameSyntax);
        var parameters = methodSymbol.Parameters;
        var i = 0;
        var scope = new ScopeContext((Scope?)parameters[i++].ExplicitDefaultValue ?? Scope._Auto, ScopeSelection.Implicit);
        var constructorSelector = R.SuccessOption((Method?)parameters[i++].ExplicitDefaultValue).Omits<SymbolErrorWithLocation>();
        var isInjectable = (bool?)parameters[i++].ExplicitDefaultValue ?? false;
        var isNewOverridable = (bool?)parameters[i++].ExplicitDefaultValue ?? false;
        var argumentIndex = 0;
        var interfaceTypes = typeArguments.Take(typeArguments.Length - 1).Select(x => analysisContext.TypeFactory.GetType(x.TypeSymbol)).ToImmutableArray();
        var implementationTypeSymbol = typeArguments.Last();
        var implementationTypeResult = analysisContext.TypeFactory.GetFullType(implementationTypeSymbol.TypeSymbol);
        if (!implementationTypeResult.TryGet(out var implementationType, out var error))
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.InfiniteRecursionError, implementationTypeSymbol, error.GetErrorText());
            return;
        }

        foreach (var argumentSyntax in node.Arguments)
        {
            if (argumentSyntax.NameColon != null)
            {
                switch (argumentSyntax.NameColon.Name.ToString())
                {
                    case nameof(scope):
                        scope = this.GetScope(argumentSyntax, implementationType.Type);
                        break;
                    case nameof(constructorSelector):
                        constructorSelector = this.GetMethod(argumentSyntax);
                        break;
                    case nameof(isInjectable):
                        isInjectable = GetBooleanLiteralValue(argumentSyntax);
                        break;
                    case nameof(isNewOverridable):
                        isNewOverridable = GetBooleanLiteralValue(argumentSyntax);
                        break;
                }
            }
            else
            {
                switch (argumentIndex)
                {
                    case 0:
                        scope = this.GetScope(argumentSyntax, implementationType.Type);
                        break;
                    case 1:
                        constructorSelector = this.GetMethod(argumentSyntax);
                        break;
                    case 2:
                        isInjectable = GetBooleanLiteralValue(argumentSyntax);
                        break;
                    case 3:
                        isNewOverridable = GetBooleanLiteralValue(argumentSyntax);
                        break;
                }

                argumentIndex++;
            }
        }

        if (constructorSelector.IsError)
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.InfiniteRecursionError, constructorSelector.Error);
            return;
        }

        var actualMethodOption = constructorSelector.Value ?? implementationType.DefaultConstructor;
        if (actualMethodOption.TryGetValue(out var actualMethod))
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.Bind(interfaceTypes, implementationType, actualMethod, scope, isInjectable, isNewOverridable);
            return;
        }

        analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.NoFactoryMethodFoundForTypeError, implementationTypeSymbol);
    }

    private static bool GetBooleanLiteralValue(ArgumentSyntax argumentSyntax)
    {
        return argumentSyntax.Expression is LiteralExpressionSyntax literalExpressionSyntax && ((bool?)literalExpressionSyntax.Token.Value ?? false);
    }

    private ScopeContext GetScope(ArgumentSyntax argumentSyntax, Symbol targetType)
    {
        return ExpressionAnalysisHelper.GetScope(analysisContext.SemanticModel, argumentSyntax, analysisContext.TypeFactory, targetType);
    }

    private R<Method?, SymbolErrorWithLocation> GetMethod(ArgumentSyntax argumentSyntax)
    {
        return ExpressionAnalysisHelper.GetMethod(argumentSyntax, analysisContext.SemanticModel, analysisContext.TypeFactory);
    }
}