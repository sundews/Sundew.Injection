// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryConstructorParameterInjectionNode.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record FactoryConstructorParameterInjectionNode : InjectionNode, IParameterNode
{
    public FactoryConstructorParameterInjectionNode(
        DefiniteType type,
        string name,
        ParameterSource parameterSource,
        TypeMetadata typeMetadata,
        InjectionNode parentInjectionNode)
        : base(parentInjectionNode)
    {
        this.Type = type;
        this.Name = name;
        this.ParameterSource = parameterSource;
        this.TypeMetadata = typeMetadata;
    }

    public DefiniteType Type { get; }

    public override string Name { get; }

    public ParameterSource ParameterSource { get; }

    public bool RequiresNewInstance => false;

    public TypeMetadata TypeMetadata { get; }
}