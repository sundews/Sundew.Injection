// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopeResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Sundew.Injection.Generator.TypeSystem;

internal class ScopeResolver(
    IReadOnlyDictionary<TypeId, ScopeResolverBuilder.ScopeContext> scopes)
{
    public Scope ResolveScope(Type type)
    {
        var scope = scopes[type.Id];
        return scope?.Scope ?? Scope._NewInstance(Location.None);
    }
}