﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHaveParametersNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;

using Sundew.Injection.Generator.TypeSystem;

internal interface IHaveParametersNode : IInjectionNode
{
    IReadOnlyRecordList<InjectionNode> Parameters { get; }
}