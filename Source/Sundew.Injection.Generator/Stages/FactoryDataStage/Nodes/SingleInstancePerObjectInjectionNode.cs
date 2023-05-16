// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleInstancePerObjectInjectionNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

using System;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Primitives.Computation;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record SingleInstancePerObjectInjectionNode(
    DefiniteType TargetType,
    DefiniteType TargetReferenceType,
    bool NeedsLifecycleHandling,
    IReadOnlyRecordList<InjectionNode> Parameters,
    CreationSource CreationSource,
    O<ParameterNode> OptionalParameterNode,
    O<ValueArray<DefiniteParameter>> OverridableNewParametersOption,
    string? ParentName) : InjectionNode(ParentName), IHaveParameters
{
    public override string Name => this.TargetType.Name;

    public bool Equals(SingleInstancePerObjectInjectionNode? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.TargetType.Equals(other.TargetType) && this.TargetReferenceType.Equals(other.TargetReferenceType) && this.Parameters.Equals(other.Parameters) && this.CreationSource.Equals(other.CreationSource) && this.OptionalParameterNode.Equals(other.OptionalParameterNode) && this.OverridableNewParametersOption.Equals(other.OverridableNewParametersOption);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.TargetType, this.TargetReferenceType, this.Parameters, this.CreationSource, this.OptionalParameterNode, this.OverridableNewParametersOption);
    }
}