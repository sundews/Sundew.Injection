// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

internal sealed record ArrayType(Type ElementType) : Type(ElementType.Name)
{
    public override TypeId Id => new TypeId($"{this.ElementType.Id}[]");
}