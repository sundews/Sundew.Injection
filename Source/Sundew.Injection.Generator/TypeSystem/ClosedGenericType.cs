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
        ValueArray<FullTypeArgument> TypeArguments,
        bool IsValueType)
    : Type(Name, Namespace, AssemblyName, IsValueType)
{
    private const string Separator = ", ";

    public override string FullName => base.FullName + '´' + this.TypeArguments.Count;

    public override TypeId Id { get; } = new($"{Name}<{TypeArguments.JoinToString((builder, x) => builder.Append(x.Type.Id), Separator)}> | {Namespace} | {AssemblyName}");
}