// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreationSource.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;

using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;
using Sundew.Injection.Generator.TypeSystem;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record CreationSource
{
    internal sealed partial record ArrayCreation(Type ElementType) : CreationSource;

    internal sealed partial record ConstructorCall(Type Type) : CreationSource;

    internal sealed partial record StaticMethodCall(Type Type, Method Method) : CreationSource;

    internal sealed partial record InstanceMethodCall(Type Type, Method Method, InjectionNode Instance, bool IsProperty) : CreationSource;

    internal sealed partial record LiteralValue(string Literal) : CreationSource;

    internal sealed partial record DefaultValue(Type Type) : CreationSource;

    internal sealed partial record IteratorMethodCall(Type ReturnType, Type ElementType) : CreationSource;
}