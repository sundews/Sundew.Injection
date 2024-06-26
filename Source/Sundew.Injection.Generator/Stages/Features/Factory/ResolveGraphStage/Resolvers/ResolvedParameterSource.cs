﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolvedParameterSource.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record ResolvedParameterSource
{
    internal sealed record Found(ParameterSource ParameterSource) : ResolvedParameterSource;

    internal sealed record NotFound(ParameterSource ProposedParameterSource) : ResolvedParameterSource;

    internal sealed record NoExactMatch(Type Type, string Name, ValueArray<ParameterSource> ParameterSources) : ResolvedParameterSource;
}