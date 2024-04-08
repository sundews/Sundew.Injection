// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryConstructorParameterInjectionNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;

using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record FactoryConstructorParameterInjectionNode(
    Type Type,
    string Name,
    ParameterSource ParameterSource,
    TypeMetadata TypeMetadata,
    string DependantName) : InjectionNode(DependantName), IParameterNode
{
    public override string Name { get; } = Name;

    public bool PrefersNewInstance => false;

    public bool IsOptional => false;

    public bool IsForConstructor => true;
}