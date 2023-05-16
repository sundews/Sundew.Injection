// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class CollectionHelper
    {
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