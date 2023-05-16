// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethodParameterInjectionNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

using System;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record FactoryMethodParameterInjectionNode : InjectionNode, IParameterNode, IEquatable<FactoryMethodParameterInjectionNode>
{
    public FactoryMethodParameterInjectionNode(DefiniteType type, string name, ParameterSource parameterSource, TypeMetadata typeMetadata, bool requiresNewInstance, string parentName)
        : base(parentName)
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

    public bool Equals(FactoryMethodParameterInjectionNode? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Type.Equals(other.Type) && this.Name == other.Name && this.ParameterSource.Equals(other.ParameterSource) && this.TypeMetadata.Equals(other.TypeMetadata) && this.RequiresNewInstance == other.RequiresNewInstance;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Type, this.Name, this.ParameterSource, this.TypeMetadata, this.RequiresNewInstance);
    }
}