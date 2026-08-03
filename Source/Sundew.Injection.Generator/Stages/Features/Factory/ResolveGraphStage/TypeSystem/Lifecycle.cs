// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lifecycle.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;

using System;

[Flags]
public enum Lifecycle
{
    None = 0,
    Initialization = 1,
    Disposal = 2,
    Both,
}