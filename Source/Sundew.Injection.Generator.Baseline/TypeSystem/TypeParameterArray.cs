﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeParameterArray.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

public sealed record TypeParameterArray(TypeParameter ElementTypeParameter) : Symbol(ElementTypeParameter);
