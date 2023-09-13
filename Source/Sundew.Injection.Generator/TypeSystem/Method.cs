// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Method.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using Sundew.Base.Collections.Immutable;

internal sealed record class Method(
    ValueArray<Parameter> Parameters,
    string Name,
    Type ContainingType,
    MethodKind Kind);