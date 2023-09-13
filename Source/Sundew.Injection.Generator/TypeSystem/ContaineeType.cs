// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContaineeType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using Sundew.Base.Collections.Immutable;

[DiscriminatedUnions.DiscriminatedUnion]
internal abstract record ContaineeType(string Name, string Namespace, string AssemblyName)
{
    public sealed record GenericType(string Name, string Namespace, string AssemblyName, ValueArray<TypeParameter> TypeParameters) : ContaineeType(Name, Namespace, AssemblyName);

    public sealed record NamedType(string Name, string Namespace, string AssemblyName) : ContaineeType(Name, Namespace, AssemblyName);
}