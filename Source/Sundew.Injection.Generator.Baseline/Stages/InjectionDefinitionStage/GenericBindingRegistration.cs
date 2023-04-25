// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericBindingRegistration.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using Sundew.Injection.Generator.TypeSystem;

internal sealed record GenericBindingRegistration(
    GenericType TargetType,
    Scope Scope,
    GenericMethod Method,
    bool ImplementsIDisposable,
    Accessibility Accessibility,
    bool IsNewOverridable);