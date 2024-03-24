// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefiniteTypeExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System.Text;
using Sundew.Base.Text;

public static class DefiniteTypeExtensions
{
    internal static string GetDefiniteTypeName(this DefiniteType definiteType)
    {
        return definiteType switch
        {
            DefiniteArrayType definiteArrayType => definiteArrayType.ElementType.Name,
            DefiniteClosedGenericType definiteBoundGenericType => GetDefiniteBoundGenericTypeName(
                definiteBoundGenericType),
            DefiniteNestedType definiteNestedType => definiteNestedType.Name,
            NamedType namedType => namedType.Name,
        };

        string GetDefiniteBoundGenericTypeName(DefiniteClosedGenericType definiteBoundGenericType)
        {
            var stringBuilder = new StringBuilder(definiteBoundGenericType.Name);
            stringBuilder.AppendItems(definiteBoundGenericType.TypeArguments, (builder, x) => builder.Append(GetDefiniteTypeName(x.Type)), string.Empty);
            return stringBuilder.ToString();
        }
    }
}