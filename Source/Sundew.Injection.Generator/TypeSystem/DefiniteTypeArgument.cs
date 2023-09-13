// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefiniteTypeArgument.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

internal readonly record struct DefiniteTypeArgument(DefiniteType Type, TypeMetadata TypeMetadata)
{
    public DefiniteTypeArgument((DefiniteType Type, TypeMetadata TypeMetadata) fullType)
        : this(fullType.Type, fullType.TypeMetadata)
    {
    }
}