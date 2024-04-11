// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionAnalysisHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Injection.Generator.TypeSystem;
using Scope = Sundew.Injection.Generator.TypeSystem.Scope;

internal static class ExpressionAnalysisHelper
{
    private const string True = "true";
    private const string False = "false";

    public static Method? GetMethod(ArgumentSyntax argumentSyntax, SemanticModel semanticModel, TypeFactory typeFactory)
    {
        if (argumentSyntax.Expression is ParenthesizedLambdaExpressionSyntax parenthesizedLambdaExpressionSyntax)
        {
            switch (parenthesizedLambdaExpressionSyntax.ExpressionBody)
            {
                case InvocationExpressionSyntax invocationExpressionSyntax:
                    var invocationSymbolInfo = semanticModel.GetSymbolInfo(invocationExpressionSyntax.Expression);
                    if (invocationSymbolInfo.Symbol is IMethodSymbol invocationMethodSymbol)
                    {
                        return typeFactory.CreateMethod(invocationMethodSymbol);
                    }

                    break;
                case ObjectCreationExpressionSyntax objectCreationExpressionSyntax:
                    var initializerSymbolInfo = semanticModel.GetSymbolInfo(objectCreationExpressionSyntax);
                    if (initializerSymbolInfo.Symbol is IMethodSymbol objectCreationMethodSymbol)
                    {
                        return typeFactory.CreateMethod(objectCreationMethodSymbol);
                    }

                    break;
                case MemberAccessExpressionSyntax memberAccessExpressionSyntax:
                    var memberAccessSymbolInfo = semanticModel.GetSymbolInfo(memberAccessExpressionSyntax);
                    switch (memberAccessSymbolInfo.Symbol)
                    {
                        case IMethodSymbol memberAccessMethodSymbol:
                            return typeFactory.CreateMethod(memberAccessMethodSymbol);
                        case IPropertySymbol memberAccessPropertySymbol:
                            return typeFactory.CreateMethod(memberAccessPropertySymbol);
                    }

                    break;
            }
        }

        return default;
    }

    public static GenericMethod GetGenericMethod(ArgumentSyntax argumentSyntax, SemanticModel semanticModel, TypeFactory typeFactory)
    {
        if (argumentSyntax.Expression is ParenthesizedLambdaExpressionSyntax parenthesizedLambdaExpressionSyntax)
        {
            switch (parenthesizedLambdaExpressionSyntax.ExpressionBody)
            {
                case InvocationExpressionSyntax invocationExpressionSyntax:
                    var invocationSymbolInfo = semanticModel.GetSymbolInfo(invocationExpressionSyntax.Expression);
                    if (invocationSymbolInfo.Symbol is IMethodSymbol invocationMethodSymbol)
                    {
                        return typeFactory.GetGenericMethod(invocationMethodSymbol.ConstructedFrom);
                    }

                    break;
                case ObjectCreationExpressionSyntax objectCreationExpressionSyntax:
                    var initializerSymbolInfo = semanticModel.GetSymbolInfo(objectCreationExpressionSyntax);
                    if (initializerSymbolInfo.Symbol is IMethodSymbol objectCreationMethodSymbol)
                    {
                        return typeFactory.GetGenericMethod(objectCreationMethodSymbol.ConstructedFrom);
                    }

                    break;
            }
        }

        return default;
    }

    public static ScopeContext GetScope(SemanticModel semanticModel, ArgumentSyntax argumentSyntax, TypeFactory typeFactory, Symbol targetType)
    {
        var symbolInfo = semanticModel.GetSymbolInfo(argumentSyntax.Expression);
        return symbolInfo.Symbol?.Name switch
        {
            nameof(Scope.SingleInstancePerFactory) => new ScopeContext(GetSingleInstancePerFactoryResult(semanticModel, argumentSyntax, targetType), ScopeSelection.Explicit),
            nameof(Scope.SingleInstancePerRequest) => new ScopeContext(Scope._SingleInstancePerRequest(argumentSyntax.GetLocation()), ScopeSelection.Explicit),
            nameof(Scope.SingleInstancePerFuncResult) => new ScopeContext(GetSingleInstancePerFuncResult(semanticModel, argumentSyntax, typeFactory), ScopeSelection.Explicit),
            nameof(Scope.NewInstance) => new ScopeContext(Scope._NewInstance(argumentSyntax.GetLocation()), ScopeSelection.Explicit),
            _ => new ScopeContext(Scope._Auto, ScopeSelection.Implicit),
        };
    }

    private static Scope GetSingleInstancePerFuncResult(SemanticModel semanticModel, ArgumentSyntax argumentSyntax, TypeFactory typeFactory)
    {
        if (argumentSyntax.Expression is InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return Scope._SingleInstancePerFuncResult(GetMethod(invocationExpressionSyntax.ArgumentList.Arguments.Single(), semanticModel, typeFactory)!, argumentSyntax.GetLocation());
        }

        return default!;
    }

    private static Scope GetSingleInstancePerFactoryResult(SemanticModel semanticModel, ArgumentSyntax argumentSyntax, Symbol type)
    {
        if (argumentSyntax.Expression is InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return GetSingleInstancePerFactory(invocationExpressionSyntax.ArgumentList.Arguments.FirstOrDefault(), semanticModel, type);
        }

        return Scope._SingleInstancePerFactory(default, Location.None);
    }

    private static Scope GetSingleInstancePerFactory(ArgumentSyntax? argumentSyntax, SemanticModel semanticModel, Symbol targetType)
    {
        return argumentSyntax?.Expression switch
        {
            LiteralExpressionSyntax literalExpressionSyntax => GetFromLiteral(literalExpressionSyntax.Token.Text),
            _ => Scope._SingleInstancePerFactory(default, argumentSyntax?.GetLocation() ?? Location.None),
        };

        Scope GetFromLiteral(string literalText)
        {
            return literalText switch
            {
                True => Scope._SingleInstancePerFactory(targetType.Name, argumentSyntax?.GetLocation() ?? Location.None),
                False => Scope._SingleInstancePerFactory(default, argumentSyntax?.GetLocation() ?? Location.None),
                ['"', .., '"'] value => Scope._SingleInstancePerFactory(value.Substring(1, value.Length - 2), argumentSyntax?.GetLocation() ?? Location.None),
                _ => Scope._SingleInstancePerFactory(default, argumentSyntax?.GetLocation() ?? Location.None),
            };
        }
    }
}