// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericBindingRegistration.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using Sundew.Injection.Generator.TypeSystem;

internal sealed record GenericBindingRegistration(
    OpenGenericType TargetType,
    ScopeContext Scope,
    GenericMethod Method,
    Accessibility Accessibility,
    bool HasLifecycle,
    bool IsNewOverridable);