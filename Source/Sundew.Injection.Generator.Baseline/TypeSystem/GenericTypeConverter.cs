// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericTypeConverter.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Base.Collections.Immutable;

public static class GenericTypeConverter
{
    public static UnboundGenericType ToUnboundGenericType(this DefiniteBoundGenericType definiteBoundGenericType)
    {
        return new UnboundGenericType(definiteBoundGenericType.Name, definiteBoundGenericType.Namespace, definiteBoundGenericType.AssemblyName);
    }

    public static UnboundGenericType ToUnboundGenericType(this GenericType genericType)
    {
        return new UnboundGenericType(genericType.Name, genericType.Namespace, genericType.AssemblyName);
    }

    public static GenericType GetGenericType(INamedTypeSymbol genericTypeSymbol)
    {
        return new GenericType(
            genericTypeSymbol.Name,
            TypeHelper.GetNamespace(genericTypeSymbol.ContainingNamespace),
            genericTypeSymbol.ContainingAssembly.Identity.ToString(),
            genericTypeSymbol.TypeParameters.Select(x => new TypeParameter(x.MetadataName)).ToImmutableArray());
    }

    public static UnboundGenericType GetUnboundGenericType(INamedTypeSymbol unboundGenericTypeSymbol)
    {
        return new UnboundGenericType(unboundGenericTypeSymbol.Name, TypeHelper.GetNamespace(unboundGenericTypeSymbol.ContainingNamespace), unboundGenericTypeSymbol.ContainingAssembly.Identity.ToString());
    }

    public static BoundGenericType ToBoundGenericType(this GenericType genericType, ValueArray<TypeArgument> typeArguments)
    {
        return new BoundGenericType(genericType.Name, genericType.Namespace, genericType.AssemblyName, genericType.TypeParameters, typeArguments);
    }

    public static DefiniteBoundGenericType ToDefiniteBoundGenericType(this GenericType genericType, ValueArray<DefiniteTypeArgument> typeArguments)
    {
        return new DefiniteBoundGenericType(genericType.Name, genericType.Namespace, genericType.AssemblyName, genericType.TypeParameters, typeArguments);
    }

    public static DefiniteBoundGenericType ToDefiniteBoundGenericType(this ContaineeType.GenericType genericType, ValueArray<DefiniteTypeArgument> typeArguments)
    {
        return new DefiniteBoundGenericType(genericType.Name, genericType.Namespace, genericType.AssemblyName, genericType.TypeParameters, typeArguments);
    }
}