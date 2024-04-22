// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FullTypeArgument.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

internal readonly record struct FullTypeArgument(Type Type, TypeMetadata TypeMetadata)
{
    public FullTypeArgument(FullType fullType)
        : this(fullType.Type, fullType.Metadata)
    {
    }
}

internal readonly record struct TypeArgument(Type Type);