// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FullParameter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

internal readonly record struct FullParameter(Type Type, string Name, TypeMetadata TypeMetadata, Method? DefaultConstructor, ParameterNecessity ParameterNecessity);