// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System.Text;
using Sundew.Base.Text;

public static class TypeExtensions
{
    internal static string GetTypeName(this Type type)
    {
        return type switch
        {
            ArrayType arrayType => arrayType.ElementType.Name,
            ClosedGenericType closedGenericType => GetClosedGenericTypeName(closedGenericType),
            NestedType nestedType => nestedType.Name,
            NamedType namedType => namedType.Name,
        };

        string GetClosedGenericTypeName(ClosedGenericType boundGenericType)
        {
            var stringBuilder = new StringBuilder(boundGenericType.Name);
            stringBuilder.AppendItems(boundGenericType.TypeArguments, (builder, x) => builder.Append(GetTypeName(x.Type)), string.Empty);
            return stringBuilder.ToString();
        }
    }
}