// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefiniteParameter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

internal readonly record struct DefiniteParameter(DefiniteType Type, string Name, TypeMetadata TypeMetadata, ParameterNecessity ParameterNecessity);