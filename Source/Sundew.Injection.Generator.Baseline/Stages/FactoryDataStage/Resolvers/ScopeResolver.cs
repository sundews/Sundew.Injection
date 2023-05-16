// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopeResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers;

using System.Collections.Generic;
using Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;
using Sundew.Injection.Generator.TypeSystem;

internal class ScopeResolver
{
    private readonly IReadOnlyDictionary<Binding, Scope> bindingScopes;
    private readonly IReadOnlyDictionary<Type, Scope> externalParameterScopes;

    public ScopeResolver(IReadOnlyDictionary<Binding, Scope> bindingScopes, IReadOnlyDictionary<Type, Scope> externalParameterScopes)
    {
        this.bindingScopes = bindingScopes;
        this.externalParameterScopes = externalParameterScopes;
    }

    public Scope ResolveScope(Binding binding)
    {
        if (!this.bindingScopes.TryGetValue(binding, out var scope))
        {
            // TODO what if not found
        }

        return scope;
    }

    public Scope ResolveScope(Type externalType)
    {
        if (!this.externalParameterScopes.TryGetValue(externalType, out var scope))
        {
            // TODO what if not found
        }

        return scope;
    }
}