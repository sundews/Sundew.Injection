// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Binding.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;

using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record Binding(
    Type TargetType,
    Type ReferencedType,
    ScopeContext Scope,
    Method Method,
    bool HasLifecycle,
    bool IsInjectable,
    bool IsNewOverridable);