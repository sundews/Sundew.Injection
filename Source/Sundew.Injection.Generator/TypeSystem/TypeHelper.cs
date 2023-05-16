// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

public static class TypeHelper
{
    public static INamedTypeSymbol? TryGetTypeByAssemblyQualifiedMetadataName(this Compilation compilation, System.Type type)
    {
        return compilation.GetTypesByMetadataName(type.FullName).FirstOrDefault(x => x.ContainingAssembly.Name == type.Assembly.GetName().Name);
    }

    public static INamedTypeSymbol? TryGetTypeByAssemblyQualifiedMetadataName(this Compilation compilation, System.Type type, string assemblyName)
    {
        return compilation.GetTypesByMetadataName(type.FullName).FirstOrDefault(x => x.ContainingAssembly.Name == assemblyName);
    }

    public static INamedTypeSymbol GetTypeByAssemblyQualifiedMetadataName(this Compilation compilation, System.Type type)
    {
        return compilation.TryGetTypeByAssemblyQualifiedMetadataName(type) ?? throw new InvalidOperationException($"Type: {type} was not found in compilation");
    }

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