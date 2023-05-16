// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

[DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record InjectionNode : IInjectionNode
{
    protected InjectionNode(string? parentName)
    {
        this.ParentName = parentName;
    }

    public abstract string Name { get; }

    public string? ParentName { get; }
}