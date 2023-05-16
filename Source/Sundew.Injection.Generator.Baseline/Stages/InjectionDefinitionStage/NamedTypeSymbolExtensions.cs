// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedTypeSymbolExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

public static class NamedTypeSymbolExtensions
{
    public static IMethodSymbol? GetMethodWithMostParameters(this ImmutableArray<IMethodSymbol> methods)
    {
        if (methods == default)
        {
            return default;
        }

        return methods.OrderByDescending(x => x.Parameters.Length).FirstOrDefault();
    }

    public static bool IsInstantiable(this ITypeSymbol typeSymbol)
    {
        return !typeSymbol.IsAbstract && typeSymbol.TypeKind == TypeKind.Class && typeSymbol.SpecialType != SpecialType.System_String;
    }

    public static bool CanBeAssignedTo(this ITypeSymbol? namedTypeSymbol, INamedTypeSymbol rhs)
    {
        while (namedTypeSymbol != null)
        {
            if (SymbolEqualityComparer.Default.Equals(namedTypeSymbol, rhs))
            {
                return true;
            }

            var any = namedTypeSymbol.Interfaces.Any(x => x.CanBeAssignedTo(rhs));
            if (any)
            {
                return true;
            }

            namedTypeSymbol = namedTypeSymbol.BaseType;
        }

        return false;
    }
}