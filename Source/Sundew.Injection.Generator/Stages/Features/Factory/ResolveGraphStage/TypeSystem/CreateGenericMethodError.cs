// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateGenericMethodError.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;

using System.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record CreateGenericMethodError(string Name, ContaineeType ContainingType, ImmutableArray<(GenericParameter TargetParameter, Symbol UnresolvedSymbol)> FailedParameters);