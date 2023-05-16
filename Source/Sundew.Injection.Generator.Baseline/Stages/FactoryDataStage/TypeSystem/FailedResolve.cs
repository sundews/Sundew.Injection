﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FailedResolve.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;

using System.Collections.Generic;
using Sundew.Injection.Generator.TypeSystem;

public sealed record FailedResolve(Type Type, IReadOnlyList<FailedResolve>? InnerFailures = null);