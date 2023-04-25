// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateMethodError.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;

using System.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

public sealed record class CreateMethodError(Type? UnresolvedContainingType, ImmutableArray<Parameter> UnresolvedParameters) : BindingError;