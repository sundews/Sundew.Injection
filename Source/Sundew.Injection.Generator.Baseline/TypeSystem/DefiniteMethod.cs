// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefiniteMethod.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using Sundew.Base.Collections.Immutable;

public readonly record struct DefiniteMethod(ValueArray<DefiniteParameter> Parameters,
    string Name,
    DefiniteType ContainingType,
    ValueArray<DefiniteTypeArgument> TypeArguments,
    bool IsConstructor);