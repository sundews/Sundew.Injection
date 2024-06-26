﻿//HintName: Sundew.Injection.CollectionHelper.cs.generated.cs
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace OverallSuccess.SundewInjection
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class CollectionHelper
    {
        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public static IReadOnlyCollection<TItem> ToReadOnly<TItem>(this IEnumerable<TItem> items)
        {
            if (items is IReadOnlyCollection<TItem> collection)
            {
                return collection;
            }

            return items.ToArray();
        }
    }
}