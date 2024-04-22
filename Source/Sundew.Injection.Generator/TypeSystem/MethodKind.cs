// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodKind.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

[DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record MethodKind
{
    public sealed record Constructor : MethodKind;

    public sealed record Static : MethodKind;

    public sealed record Instance(TypeMetadata ContainingTypeMetadata, bool IsProperty, Method? ContainingTypeDefaultConstructor) : MethodKind;
}