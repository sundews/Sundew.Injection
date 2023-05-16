// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessorProperty.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using Sundew.Injection.Generator.TypeSystem;

public readonly record struct AccessorProperty(DefiniteType ContainingType, (Type Type, TypeMetadata TypeMetadata) Result, (Type Type, TypeMetadata TypeMetadata) Property, string Name);