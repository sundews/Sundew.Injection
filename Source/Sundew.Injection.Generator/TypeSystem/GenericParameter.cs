// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericParameter.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

public readonly record struct GenericParameter(Symbol Type, string Name, TypeMetadata TypeMetadata);