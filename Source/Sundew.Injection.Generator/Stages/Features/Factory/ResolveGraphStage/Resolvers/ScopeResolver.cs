// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopeResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

using System.Collections.Generic;
using Sundew.Injection.Generator.TypeSystem;

internal class ScopeResolver
{
    private readonly IReadOnlyDictionary<TypeId, ScopeContext> scopes;

    public ScopeResolver(
        IReadOnlyDictionary<TypeId, ScopeContext> scopes)
    {
        this.scopes = scopes;
    }

    public Scope ResolveScope(Type type)
    {
        if (!this.scopes.TryGetValue(type.Id, out var scope))
        {
            // TODO what if not found
        }

        return scope?.Scope ?? Scope._NewInstance;
    }
}