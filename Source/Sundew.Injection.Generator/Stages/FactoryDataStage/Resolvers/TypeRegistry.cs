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
    private readonly Dictionary<TypeId, Reference> registry = new();

    public void Register(TypeId targetType, TypeId? interfaceType, TValue value, bool allowOverwrite)
    {
        this.AddOrUpdate(targetType, value, allowOverwrite);
        if (interfaceType.HasValue && targetType != interfaceType)
        {
            this.AddOrUpdate(interfaceType.Value, value, allowOverwrite);
        }
    }

    public bool TryGet(TypeId type, [NotNullWhen(true)] out TValue? value)
    {
        if (this.registry.TryGetValue(type, out var reference))
        {
            value = reference.Value!;
            return true;
        }

        value = default;
        return false;
    }

    private void AddOrUpdate(TypeId targetType, TValue value, bool allowOverwrite)
    {
        if (this.registry.TryGetValue(targetType, out var reference))
        {
            if (allowOverwrite)
            {
                reference.Value = value;
            }
        }
        else
        {
            this.registry[targetType] = new Reference(value);
        }
    }

    private class Reference
    {
        public Reference(TValue value)
        {
            this.Value = value;
        }

        public TValue Value { get; set; }
    }
}