// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewInstanceInjectionNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

using System.Collections.Generic;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record NewInstanceInjectionNode(
    DefiniteType TargetType,
    DefiniteType TargetReferenceType,
    bool TargetImplementsDisposable,
    IReadOnlyList<InjectionNode> Parameters,
    CreationSource CreationSource,
    ParameterNode? ParameterNodeOption,
    ValueArray<DefiniteParameter>? OverridableNewParametersOption,
    InjectionNode? ParentCreationNode) : InjectionNode(ParentCreationNode), IHaveParameters
{
    public override string Name => this.TargetType.Name;
}