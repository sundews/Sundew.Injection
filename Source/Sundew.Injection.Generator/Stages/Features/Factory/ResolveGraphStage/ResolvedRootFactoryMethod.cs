// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolvedRootFactoryMethod.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record ResolvedRootFactoryMethod(
    bool IsPartialDefinition,
    bool IsProperty,
    string Name,
    FullType Return,
    FullType Target,
    InjectionNode InjectionTree,
    ValueArray<FullParameter> Parameters,
    Lifecycle RootLifecycle);
