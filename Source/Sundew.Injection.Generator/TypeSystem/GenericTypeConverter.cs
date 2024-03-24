// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericTypeConverter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Base.Collections.Immutable;

internal static class GenericTypeConverter
{
    public static UnboundGenericType ToUnboundGenericType(this DefiniteClosedGenericType definiteClosedGenericType)
    {
        return new UnboundGenericType(definiteClosedGenericType.Name, definiteClosedGenericType.TypeParameters.Count, definiteClosedGenericType.Namespace, definiteClosedGenericType.AssemblyName);
    }

    public static UnboundGenericType ToUnboundGenericType(this OpenGenericType openGenericType)
    {
        return new UnboundGenericType(openGenericType.Name, openGenericType.TypeParameters.Count, openGenericType.Namespace, openGenericType.AssemblyName);
    }

    public static UnboundGenericType ToUnboundGenericType(this DefiniteArrayType definiteArrayType)
    {
        return new UnboundGenericType(definiteArrayType.Name, 1, definiteArrayType.Namespace, definiteArrayType.AssemblyName);
    }

    public static OpenGenericType ToOpenGenericType(this ClosedGenericType closedGenericType)
    {
        return new OpenGenericType(closedGenericType.Name, closedGenericType.Namespace, closedGenericType.AssemblyName, closedGenericType.TypeParameters, closedGenericType.IsValueType);
    }

    public static OpenGenericType GetGenericType(INamedTypeSymbol genericTypeSymbol)
    {
        return new OpenGenericType(
            genericTypeSymbol.Name,
            TypeHelper.GetNamespace(genericTypeSymbol.ContainingNamespace),
            genericTypeSymbol.ContainingAssembly.Identity.ToString(),
            genericTypeSymbol.TypeParameters.Select(x => new TypeParameter(x.MetadataName)).ToImmutableArray(),
            genericTypeSymbol.IsValueType);
    }

    public static UnboundGenericType GetUnboundGenericType(INamedTypeSymbol unboundGenericTypeSymbol)
    {
        return new UnboundGenericType(unboundGenericTypeSymbol.Name, unboundGenericTypeSymbol.TypeParameters.Length, TypeHelper.GetNamespace(unboundGenericTypeSymbol.ContainingNamespace), unboundGenericTypeSymbol.ContainingAssembly.Identity.ToString());
    }

    public static DefiniteClosedGenericType ToDefiniteClosedGenericType(this OpenGenericType openGenericType, ValueArray<DefiniteTypeArgument> typeArguments)
    {
        return new DefiniteClosedGenericType(openGenericType.Name, openGenericType.Namespace, openGenericType.AssemblyName, openGenericType.TypeParameters, typeArguments, openGenericType.IsValueType);
    }

    public static DefiniteClosedGenericType ToDefiniteClosedGenericType(this ContaineeType.GenericType genericType, ValueArray<DefiniteTypeArgument> typeArguments)
    {
        return new DefiniteClosedGenericType(genericType.Name, genericType.Namespace, genericType.AssemblyName, genericType.TypeParameters, typeArguments, genericType.IsValueType);
    }
}