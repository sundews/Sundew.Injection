// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;

[DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record InjectionNode(string? DependeeName) : IInjectionNode
{
    public abstract string Name { get; }
}