﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingRegistration.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using Sundew.Injection.Generator.TypeSystem;

internal sealed record BindingRegistration(
    FullType TargetType,
    Type ReferencedType,
    ScopeContext Scope,
    Method Method,
    bool IsInjectable,
    bool IsNewOverridable);
