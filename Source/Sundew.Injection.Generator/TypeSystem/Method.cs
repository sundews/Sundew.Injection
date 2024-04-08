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
    ValueArray<Parameter> Parameters,
    ValueArray<TypeArgument> TypeArguments,
    MethodKind Kind)
{
    public Method(
        Type containingType,
        string name,
        MethodKind kind)
        : this(containingType, name, ValueArray<Parameter>.Empty, ValueArray<TypeArgument>.Empty, kind)
    {
    }
}