// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleInstancePerRequestInjectionNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

using Sundew.Base.Collections.Immutable;
using Sundew.Base.Primitives.Computation;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record SingleInstancePerRequestInjectionNode(
    DefiniteType TargetType,
    DefiniteType TargetReferenceType,
    bool NeedsLifecycleHandling,
    IReadOnlyRecordList<InjectionNode> Parameters,
    CreationSource CreationSource,
    O<ParameterNode> ParameterNodeOption,
    O<ValueArray<DefiniteParameter>> OverridableNewParametersOption,
    string? ParentName) : InjectionNode(ParentName), IHaveParametersNode, IMayOverrideNewNode
{
    public override string Name => this.TargetType.Name;
}