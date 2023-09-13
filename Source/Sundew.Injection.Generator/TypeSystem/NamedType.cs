// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

internal sealed record NamedType(string Name, string Namespace, string AssemblyName) : DefiniteType(Name, Namespace, AssemblyName)
{
    public override TypeId Id => new($"{this.AssemblyName}::{this.Namespace}.{this.Name}");
}