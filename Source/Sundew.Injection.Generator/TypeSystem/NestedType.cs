// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NestedType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

internal sealed record NestedType(Type ContainedType, Type ContainingType) : Type(ContainedType.Name, ContainedType.IsValueType)
{
    public override TypeId Id => this.ContainedType.Id;
}