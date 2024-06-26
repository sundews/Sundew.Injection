﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

internal sealed record ArrayType(Type ElementType)
    : Type(
        ElementType.Name,
        ElementType.Namespace,
        ElementType.AssemblyName,
        false)
{
    public override string FullName => $"{this.Namespace}.{this.Name}[]";

    public override TypeId Id { get; } = new($"{ElementType.Name}[] | {ElementType.Namespace} | {ElementType.AssemblyName}");
}