// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClosedGenericType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using Sundew.Base.Collections.Immutable;
using Sundew.Base.Text;

internal sealed record ClosedGenericType(
        string Name,
        string Namespace,
        string AssemblyName,
        ValueArray<TypeParameter> TypeParameters,
        ValueArray<TypeArgument> TypeArguments,
        bool IsValueType)
    : Type(Name, Namespace, AssemblyName, IsValueType)
{
    public override TypeId Id => new($"{this.AssemblyName}::{this.Namespace}.{this.Name}<{this.TypeArguments.JoinToString((builder, x) => builder.Append(x.Type.Id.Id), ", ")}>");
}