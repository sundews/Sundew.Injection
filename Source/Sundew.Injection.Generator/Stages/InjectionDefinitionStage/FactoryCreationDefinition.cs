// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryCreationDefinition.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using Microsoft.CodeAnalysis;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;
using Accessibility = Sundew.Injection.Accessibility;

internal readonly record struct FactoryCreationDefinition(
    NamedType FactoryType,
    NamedType? FactoryInterfaceType,
    ValueArray<FactoryMethodRegistration> FactoryMethodRegistrations,
    Accessibility Accessibility,
    Location Location);
