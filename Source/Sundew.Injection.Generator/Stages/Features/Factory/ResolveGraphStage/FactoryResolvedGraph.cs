// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryResolvedGraph.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record FactoryResolvedGraph(
    NamedType FactoryType,
    NamedType? FactoryInterfaceType,
    DeclaredConstructor DeclaredConstructor,
    Accessibility Accessibility,
    Lifecycle Lifecycle,
    InjectionTree? LifecycleHandlingInjectionTree,
    ValueDictionary<RequestedParameter, bool> ReferencedTypesOptionality,
    ValueDictionary<NamedType, ValueArray<ResolvedRootFactoryMethod>> ResolvedRootFactoryMethods);