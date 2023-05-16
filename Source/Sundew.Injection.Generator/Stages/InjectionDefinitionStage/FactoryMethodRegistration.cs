// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethodRegistration.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using Sundew.Injection.Generator.TypeSystem;
using Scope = Sundew.Injection.Scope;

internal readonly record struct FactoryMethodRegistration(
    (Type Type, TypeMetadata TypeMetadata) Return,
    (Type Type, TypeMetadata TypeMetadata) Target,
    Scope Scope,
    Method Method,
    Accessibility Accessibility,
    bool IsNewOverridable,
    string? CreateMethodName);