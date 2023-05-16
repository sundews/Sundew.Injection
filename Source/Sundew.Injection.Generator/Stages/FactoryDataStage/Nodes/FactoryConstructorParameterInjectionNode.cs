// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryConstructorParameterInjectionNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

using System;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record FactoryConstructorParameterInjectionNode : InjectionNode, IParameterNode
{
    public FactoryConstructorParameterInjectionNode(
        DefiniteType type,
        string name,
        ParameterSource parameterSource,
        TypeMetadata typeMetadata,
        string parentName)
        : base(parentName)
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

    public bool Equals(FactoryConstructorParameterInjectionNode? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Type.Equals(other.Type) && this.Name == other.Name && this.ParameterSource.Equals(other.ParameterSource) && this.TypeMetadata.Equals(other.TypeMetadata);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Type, this.Name, this.ParameterSource, this.TypeMetadata);
    }
}