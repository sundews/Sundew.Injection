// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableMetadata.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

internal readonly record struct EnumerableMetadata(bool ImplementsIEnumerable, bool IsArrayCompatible, bool IsArrayRequired)
{
    public EnumerableMetadata(bool implementsIEnumerable, (bool IsArrayCompatible, bool IsArrayRequired) arrayMetadata)
        : this(implementsIEnumerable, arrayMetadata.IsArrayCompatible, arrayMetadata.IsArrayRequired)
    {
    }

    public static EnumerableMetadata NonEnumerableMetadata { get; } = new EnumerableMetadata(false, false, false);
}