// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

internal static class TypeHelper
{
    private const string Dot = ".";

    public static string GetNamespace(INamespaceSymbol namespaceSymbol)
    {
        var @namespace = namespaceSymbol;
        var stringBuilder = new StringBuilder(@namespace.Name);
        @namespace = @namespace.ContainingNamespace;
        while (@namespace != null)
        {
            var nextNamespace = @namespace.ContainingNamespace;
            if (nextNamespace != null)
            {
                stringBuilder.Insert(0, Dot);
                stringBuilder.Insert(0, @namespace.Name);
            }

            @namespace = nextNamespace;
        }

        return stringBuilder.ToString();
    }

    public static IMethodSymbol? GetDefaultConstructorMethod(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            return GetDefaultMethodWithMostParameters(namedTypeSymbol.Constructors);
        }

        return default;
    }

    public static IMethodSymbol? GetDefaultMethodWithMostParameters(this ImmutableArray<IMethodSymbol> methods)
    {
        if (methods == default)
        {
            return default;
        }

        return methods
            .OrderByDescending(x => x.Parameters.Length)
            .SkipWhile(x =>
                x.IsStatic ||
                x.DeclaredAccessibility != Accessibility.Public ||
                (x.ContainingType.IsRecord &&
                SymbolEqualityComparer.Default.Equals(x.Parameters.FirstOrDefault()?.Type, x.ContainingType)))
            .FirstOrDefault();
    }
}