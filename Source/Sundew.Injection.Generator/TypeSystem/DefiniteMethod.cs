﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefiniteMethod.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using Sundew.Base.Collections.Immutable;

internal readonly record struct DefiniteMethod(
    DefiniteType ContainingType,
    string Name,
    ValueArray<DefiniteParameter> Parameters,
    ValueArray<DefiniteTypeArgument> TypeArguments,
    MethodKind Kind);