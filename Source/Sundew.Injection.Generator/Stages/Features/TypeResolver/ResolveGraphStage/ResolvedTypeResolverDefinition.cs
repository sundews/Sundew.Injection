﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolvedTypeResolverDefinition.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.TypeResolver.ResolveGraphStage;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal readonly record struct ResolvedTypeResolverDefinition(
    NamedType ResolverType,
    bool GenerateInterface,
    ValueArray<ResolvedFactoryRegistration> FactoryRegistrations,
    Accessibility Accessibility);