﻿namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class MethodSymbolExtensions
{
    public static TypeSymbolWithLocation[] MapTypeArguments(this IMethodSymbol methodSymbol, GenericNameSyntax genericNameSyntax)
    {
        return methodSymbol.TypeArguments.Zip(genericNameSyntax.TypeArgumentList.Arguments, (symbol, syntax) => new TypeSymbolWithLocation(symbol, syntax.GetLocation())).ToArray();
    }
}