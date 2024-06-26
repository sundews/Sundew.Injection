﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolvedBindingError.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record ResolvedBindingError : ResolvedBinding;

internal sealed record ParameterError(Type Type, string ParameterName, ValueArray<ParameterSource> ParameterSources) : ResolvedBindingError;

internal sealed record ScopeError(Type CurrentType, Scope CurrentScope, Dependant Dependant) : ResolvedBindingError;

internal sealed record CreateGenericMethodError(TypeSystem.CreateGenericMethodError Error) : ResolvedBindingError;
