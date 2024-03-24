// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICache.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features;

using System.Diagnostics.CodeAnalysis;

public interface ICache<in TKey, TValue>
{
    bool TryGet(TKey key, [NotNullWhen(true)] out TValue? value);
}