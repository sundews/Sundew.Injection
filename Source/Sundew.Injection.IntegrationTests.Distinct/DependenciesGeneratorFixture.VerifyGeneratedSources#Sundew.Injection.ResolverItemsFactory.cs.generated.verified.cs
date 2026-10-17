//HintName: Sundew.Injection.ResolverItemsFactory.cs.generated.cs
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolverItemsFactory.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace DistinctSuccess.SundewInjection
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class ResolverItemsFactory
    {
        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        internal static ResolverItem[] Create(int bucketSize, params ResolverItem[] resolvers)
        {
            if (resolvers.Length >= bucketSize)
            {
                throw new NotSupportedException("The bucket size must be larger than the number of resolvers.");
            }

            var resolverItems = new ResolverItem[bucketSize];
            for (var i = 0; i < resolvers.Length; i++)
            {
                var resolverItem = resolvers[i];
                var index = GetStartIndex(resolverItem.Type, bucketSize);
                while (resolverItems[index].Type != default)
                {
                    index++;
                    if (index >= resolverItems.Length)
                    {
                        index = 0;
                    }
                }

                resolverItems[index] = resolverItem;
            }

            return resolverItems;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        internal static int GetStartIndex(Type type, int bucketSize)
        {
            return (RuntimeHelpers.GetHashCode(type) & 0x7FFFFFFF) % bucketSize;
        }
    }
}
