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
using Sundew.Base;
using Sundew.Injection.Generator.TypeSystem;
using Scope = Sundew.Injection.Generator.TypeSystem.Scope;

internal static class ExpressionAnalysisHelper
{
    public static R<Method?, ErrorWithLocation> GetMethod(ArgumentSyntax argumentSyntax, SemanticModel semanticModel, TypeFactory typeFactory)
    {
        if (argumentSyntax.Expression is ParenthesizedLambdaExpressionSyntax parenthesizedLambdaExpressionSyntax)
        {
            switch (parenthesizedLambdaExpressionSyntax.ExpressionBody)
            {
                case InvocationExpressionSyntax invocationExpressionSyntax:
                    var invocationSymbolInfo = semanticModel.GetSymbolInfo(invocationExpressionSyntax.Expression);
                    if (invocationSymbolInfo.Symbol is IMethodSymbol invocationMethodSymbol)
                    {
                        return typeFactory.GetFactoryMethod(invocationMethodSymbol)
                            .MapError(x => new ErrorWithLocation(x, argumentSyntax.GetLocation()))
                            .MapToOptionResult();
                    }

                    break;
                case ObjectCreationExpressionSyntax objectCreationExpressionSyntax:
                    var initializerSymbolInfo = semanticModel.GetSymbolInfo(objectCreationExpressionSyntax);
                    if (initializerSymbolInfo.Symbol is IMethodSymbol objectCreationMethodSymbol)
                    {
                        return typeFactory.GetFactoryMethod(objectCreationMethodSymbol)
                            .MapError(x => new ErrorWithLocation(x, argumentSyntax.GetLocation()))
                            .MapToOptionResult();
                    }

                    break;
                case MemberAccessExpressionSyntax memberAccessExpressionSyntax:
                    var memberAccessSymbolInfo = semanticModel.GetSymbolInfo(memberAccessExpressionSyntax);
                    switch (memberAccessSymbolInfo.Symbol)
                    {
                        case IMethodSymbol memberAccessMethodSymbol:
                            return typeFactory.GetFactoryMethod(memberAccessMethodSymbol)
                                .MapError(x => new ErrorWithLocation(x, argumentSyntax.GetLocation()))
                                .MapToOptionResult();
                        case IPropertySymbol memberAccessPropertySymbol:
                            return typeFactory.GetFactoryMethod(memberAccessPropertySymbol).MapError(x => new ErrorWithLocation(x, argumentSyntax.GetLocation())).MapToOptionResult();
                    }

                    break;
            }
        }

        return R.SuccessOption<Method>();
    }

    public static R<GenericMethod, ErrorWithLocation> GetGenericMethod(ArgumentSyntax argumentSyntax, SemanticModel semanticModel, TypeFactory typeFactory)
    {
        if (argumentSyntax.Expression is ParenthesizedLambdaExpressionSyntax parenthesizedLambdaExpressionSyntax)
        {
            switch (parenthesizedLambdaExpressionSyntax.ExpressionBody)
            {
                case InvocationExpressionSyntax invocationExpressionSyntax:
                    var invocationSymbolInfo = semanticModel.GetSymbolInfo(invocationExpressionSyntax.Expression);
                    if (invocationSymbolInfo.Symbol is IMethodSymbol invocationMethodSymbol)
                    {
                        return typeFactory.GetGenericMethod(invocationMethodSymbol.ConstructedFrom).MapError(x => new ErrorWithLocation(x, argumentSyntax.GetLocation()));
                    }

                    break;
                case ObjectCreationExpressionSyntax objectCreationExpressionSyntax:
                    var initializerSymbolInfo = semanticModel.GetSymbolInfo(objectCreationExpressionSyntax);
                    if (initializerSymbolInfo.Symbol is IMethodSymbol objectCreationMethodSymbol)
                    {
                        return typeFactory.GetGenericMethod(objectCreationMethodSymbol.ConstructedFrom).MapError(x => new ErrorWithLocation(x, argumentSyntax.GetLocation()));
                    }

                    break;
            }
        }

        return R.SuccessOption<GenericMethod>();
    }

    public static ScopeContext GetScope(SemanticModel semanticModel, ArgumentSyntax argumentSyntax, TypeFactory typeFactory, Symbol targetType)
    {
        var symbolInfo = semanticModel.GetSymbolInfo(argumentSyntax.Expression);
        return symbolInfo.Symbol?.Name switch
        {
            nameof(Scope.SingleInstancePerFactory) => new ScopeContext(Scope._SingleInstancePerFactory(Location.None), ScopeSelection.Explicit),
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
            var methodResult = GetMethod(invocationExpressionSyntax.ArgumentList.Arguments.Single(), semanticModel, typeFactory);
            if (methodResult.IsError)
            {
                return default!;
            }

            return Scope._SingleInstancePerFuncResult(methodResult.Value!, argumentSyntax.GetLocation());
        }

        return default!;
    }
}