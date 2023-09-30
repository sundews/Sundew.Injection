// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContaineeType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using Sundew.Base.Collections.Immutable;

[DiscriminatedUnions.DiscriminatedUnion]
internal abstract record ContaineeType(string Name, string Namespace, string AssemblyName, bool IsValueType)
{
    public sealed record GenericType(string Name, string Namespace, string AssemblyName, ValueArray<TypeParameter> TypeParameters, bool IsValueType) : ContaineeType(Name, Namespace, AssemblyName, IsValueType);

    public sealed record NamedType(string Name, string Namespace, string AssemblyName, bool IsValueType) : ContaineeType(Name, Namespace, AssemblyName, IsValueType);
}