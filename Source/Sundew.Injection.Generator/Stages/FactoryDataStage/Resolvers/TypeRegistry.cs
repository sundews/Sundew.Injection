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

internal sealed class TypeRegistry<TValue> : ICache<TypeId, TValue>, ITypeRegistrar<TValue>
{
    private readonly Dictionary<TypeId, TValue> registry = new();

    public void Register(TypeId targetType, TypeId? interfaceType, TValue value)
    {
        this.registry[targetType] = value;
        if (interfaceType.HasValue && targetType != interfaceType)
        {
            this.registry[interfaceType.Value] = value;
        }
    }

    public bool TryGet(TypeId type, [NotNullWhen(true)] out TValue? value)
    {
        return this.registry.TryGetValue(type, out value);
    }
}