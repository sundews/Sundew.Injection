// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethodParameterInjectionNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record FactoryMethodParameterInjectionNode : InjectionNode, IParameterNode
{
    public FactoryMethodParameterInjectionNode(DefiniteType type, string name, ParameterSource parameterSource, TypeMetadata typeMetadata, bool requiresNewInstance, InjectionNode parentInjectionNode)
        : base(parentInjectionNode)
    {
        this.Type = type;
        this.Name = name;
        this.ParameterSource = parameterSource;
        this.TypeMetadata = typeMetadata;
        this.RequiresNewInstance = requiresNewInstance;
    }

    public DefiniteType Type { get; }

    public override string Name { get; }

    public ParameterSource ParameterSource { get; }

    public TypeMetadata TypeMetadata { get; }

    public bool RequiresNewInstance { get; }
}