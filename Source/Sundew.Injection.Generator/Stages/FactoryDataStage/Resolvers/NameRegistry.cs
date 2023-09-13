// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NameRegistry.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public class NameRegistry<TValue> : ICache<string, TValue>, INameRegistrar<TValue>
{
    private readonly Dictionary<string, TValue> registry = new();

    public void Register(string name, TValue value)
    {
        this.registry.Add(name, value);
    }

    public bool TryGet(string name, [NotNullWhen(true)] out TValue? value)
    {
        return this.registry.TryGetValue(name, out value);
    }
}