// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Method.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using Sundew.Base.Collections.Immutable;

internal sealed record Method(
    Type ContainingType,
    string Name,
    ValueArray<FullParameter> Parameters,
    ValueArray<FullTypeArgument> TypeArguments,
    MethodKind Kind) : ISymbol
{
    public Method(
        Type containingType,
        string name,
        MethodKind kind)
        : this(containingType, name, ValueArray<FullParameter>.Empty, ValueArray<FullTypeArgument>.Empty, kind)
    {
    }

    public string FullName => this.ContainingType.FullName + '.' + this.Name;
}