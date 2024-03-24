// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolverCreationDefinition.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using Sundew.Base.Collections.Immutable;

internal readonly record struct ResolverCreationDefinition(
    string? FactoryClassNamespace,
    string? FactoryClassName,
    bool GenerateInterface,
    ValueArray<FactoryRegistration> FactoryRegistrations,
    Accessibility Accessibility);
