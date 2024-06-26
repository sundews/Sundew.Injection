﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleInstancePerFactoryInjectionNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record SingleInstancePerFactoryInjectionNode(
    Type TargetType,
    Type ReferencedType,
    bool NeedsLifecycleHandling,
    IReadOnlyRecordList<InjectionNode> Parameters,
    CreationSource CreationSource,
    ParameterNode? ParameterNodeOption,
    ValueArray<FullParameter>? OverridableNewParametersOption,
    string? ParentName) : InjectionNode(ParentName), IHaveParametersNode, IMayOverrideNewNode
{
    public override string Name => this.TargetType.Name;
}