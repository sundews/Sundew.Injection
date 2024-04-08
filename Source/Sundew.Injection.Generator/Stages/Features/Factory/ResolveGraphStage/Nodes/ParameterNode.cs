// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;

using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal record ParameterNode(
    Type Type,
    ParameterSource ParameterSource,
    string Name,
    TypeMetadata TypeMetadata,
    bool PrefersNewInstance,
    bool IsForConstructor,
    string? DependantName) : IParameterNode
{
    public bool IsOptional => true;
}