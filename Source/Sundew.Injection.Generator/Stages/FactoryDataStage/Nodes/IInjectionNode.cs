// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjectionNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

/// <summary>
/// Interface for CreationNodes.
/// </summary>
internal interface IInjectionNode
{
    /// <summary>
    /// Gets the parent name.
    /// </summary>
    /// <value>
    /// The parent name.
    /// </value>
    string? ParentName { get; }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    string Name { get; }
}