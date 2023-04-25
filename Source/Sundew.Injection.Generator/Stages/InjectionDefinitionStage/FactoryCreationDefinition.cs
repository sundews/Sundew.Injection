// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryCreationDefinition.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using Sundew.Base.Collections.Immutable;

internal readonly record struct FactoryCreationDefinition(
    string? FactoryClassNamespace,
    string? FactoryClassName,
    bool GenerateInterface,
    ValueArray<FactoryMethodRegistration> FactoryMethodRegistrations,
    Accessibility Accessibility);
