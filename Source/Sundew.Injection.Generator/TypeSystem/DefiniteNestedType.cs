// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefiniteNestedType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

internal sealed record DefiniteNestedType(DefiniteType ContainedType, DefiniteType ContainingType) : DefiniteType($"{ContainingType.Name}.{ContainedType.Name}", ContainedType.Namespace, ContainedType.AssemblyName, ContainedType.IsValueType)
{
    public override TypeId Id => this.ContainedType.Id;
}