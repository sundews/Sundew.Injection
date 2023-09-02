// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefiniteType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using Sundew.DiscriminatedUnions;

[DiscriminatedUnion]
internal abstract partial record DefiniteType(string Name, string Namespace, string AssemblyName) : Type(Name)
{
    public virtual string FullName => $"{this.Namespace}.{this.Name}";

    public string AssemblyQualifiedName => $"{this.FullName}, {this.AssemblyName}";
}