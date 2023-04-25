// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IParameterNode.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

/// <summary>
/// Interface for parameter nodes.
/// </summary>
internal interface IParameterNode : IInjectionNode
{
    DefiniteType Type { get; }

    TypeMetadata TypeMetadata { get; }

    ParameterSource ParameterSource { get; }

    bool RequiresNewInstance { get; }
}