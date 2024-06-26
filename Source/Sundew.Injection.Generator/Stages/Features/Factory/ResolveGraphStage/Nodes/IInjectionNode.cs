﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjectionNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;

/// <summary>
/// Interface for CreationNodes.
/// </summary>
internal interface IInjectionNode
{
    /// <summary>
    /// Gets the name for the node depending on this instance.
    /// </summary>
    /// <value>
    /// The dependant name.
    /// </value>
    string? DependantName { get; }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    string Name { get; }
}