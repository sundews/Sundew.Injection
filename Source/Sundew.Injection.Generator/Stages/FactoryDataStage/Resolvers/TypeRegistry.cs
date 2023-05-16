// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeRegistry.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sundew.Injection.Generator.TypeSystem;

public class TypeRegistry<TValue> : ICache<Type, TValue>, ITypeRegistrar<TValue>
{
    private readonly Dictionary<Type, TValue> registry = new Dictionary<Type, TValue>();

    public void Register(Type targetType, Type? interfaceType, TValue value)
    {
        this.registry[targetType] = value;
        if (interfaceType != null && targetType != interfaceType)
        {
            this.registry[interfaceType] = value;
        }
    }

    public bool TryGet(Type type, [NotNullWhen(true)] out TValue? value)
    {
        return this.registry.TryGetValue(type, out value);
    }
}