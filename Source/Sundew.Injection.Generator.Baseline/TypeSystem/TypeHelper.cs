// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

public static class TypeHelper
{
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
                stringBuilder.Insert(0, ".");
                stringBuilder.Insert(0, @namespace.Name);
            }

            @namespace = nextNamespace;
        }

        return stringBuilder.ToString();
    }

    public static IMethodSymbol GetDefaultConstructorMethod(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.Constructors.OrderByDescending(x => x.Parameters.Length).First();
        }

        return default!;
    }
}