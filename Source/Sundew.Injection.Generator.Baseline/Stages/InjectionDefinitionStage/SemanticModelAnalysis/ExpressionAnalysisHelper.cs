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

    public static Scope GetScope(SemanticModel semanticModel, ArgumentSyntax argumentSyntax, TypeFactory typeFactory)
    {
        var symbolInfo = semanticModel.GetSymbolInfo(argumentSyntax.Expression);
        return symbolInfo.Symbol?.Name switch
        {
            nameof(Scope.SingleInstancePerFactory) => Scope.SingleInstancePerFactory,
            nameof(Scope.SingleInstancePerRequest) => Scope.SingleInstancePerRequest,
            nameof(Scope.SingleInstancePerFuncResult) => GetSingleInstancePerFuncResult(semanticModel, argumentSyntax, typeFactory),
            nameof(Scope.NewInstance) => Scope.NewInstance,
            _ => Scope.Auto,
        };
    }

    private static Scope GetSingleInstancePerFuncResult(SemanticModel semanticModel, ArgumentSyntax argumentSyntax, TypeFactory typeFactory)
    {
        if (argumentSyntax.Expression is InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return Scope.SingleInstancePerFuncResult(GetMethod(invocationExpressionSyntax.ArgumentList.Arguments.Single(), semanticModel, typeFactory)!);
        }

        return null!;
    }
}