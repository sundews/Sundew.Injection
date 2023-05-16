﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHaveParameters.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

using Sundew.Injection.Generator.TypeSystem;

internal interface IHaveParameters : IInjectionNode
{
    IReadOnlyRecordList<InjectionNode> Parameters { get; }
}