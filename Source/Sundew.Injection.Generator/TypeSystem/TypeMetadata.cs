﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainingTypeMetadata.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

internal readonly record struct TypeMetadata(EnumerableMetadata EnumerableMetadata, bool HasLifecycle);