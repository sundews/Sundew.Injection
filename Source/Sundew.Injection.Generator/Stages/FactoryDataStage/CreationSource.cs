// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreationSource.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage;

using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;
using Sundew.Injection.Generator.TypeSystem;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record CreationSource
{
    internal sealed record ArrayCreation(DefiniteType ElementType) : CreationSource;

    internal sealed record ConstructorCall(DefiniteType Type) : CreationSource;

    internal sealed record StaticMethodCall(DefiniteType Type, DefiniteMethod Method) : CreationSource;

    internal sealed record InstanceMethodCall(DefiniteType Type, DefiniteMethod Method, InjectionNode Instance, bool IsProperty) : CreationSource;

    internal sealed record LiteralValue(string Literal) : CreationSource;

    internal sealed record DefaultValue(DefiniteType DefiniteType) : CreationSource;

    internal sealed record IteratorMethodCall(DefiniteType ElementType) : CreationSource;
}