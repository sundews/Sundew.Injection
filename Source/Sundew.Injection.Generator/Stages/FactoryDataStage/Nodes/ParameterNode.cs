namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

using System;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal record ParameterNode(
    DefiniteType Type,
    ParameterSource ParameterSource,
    string Name,
    TypeMetadata TypeMetadata,
    bool RequiresNewInstance,
    string? ParentName) : IParameterNode
{
    public virtual bool Equals(ParameterNode? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Type.Equals(other.Type) && this.ParameterSource.Equals(other.ParameterSource) && this.Name == other.Name && this.TypeMetadata.Equals(other.TypeMetadata) && this.RequiresNewInstance == other.RequiresNewInstance;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Type, this.ParameterSource, this.Name, this.TypeMetadata, this.RequiresNewInstance);
    }
}