// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal record ParameterNode(
    DefiniteType Type,
    ParameterSource ParameterSource,
    string Name,
    TypeMetadata TypeMetadata,
    bool RequiresNewInstance,
    bool IsForConstructor,
    string? DependeeName) : IParameterNode
{
    public bool IsOptional => true;
}