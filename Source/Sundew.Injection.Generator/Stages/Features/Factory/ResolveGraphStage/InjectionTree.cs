// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionTree.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

internal sealed record InjectionTree(InjectionNode Root, ValueList<FactoryConstructorParameter> FactoryConstructorParameters, bool NeedsLifecycleHandling, bool RootNeedsLifecycleHandling);