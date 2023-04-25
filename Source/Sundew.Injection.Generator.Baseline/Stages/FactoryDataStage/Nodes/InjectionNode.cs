// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionNode.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

[DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record InjectionNode : IInjectionNode
{
    protected InjectionNode(InjectionNode? parentInjectionNode)
    {
        this.ParentInjectionNode = parentInjectionNode;
    }

    public abstract string Name { get; }

    public InjectionNode? ParentInjectionNode { get; }
}