//HintName: Sundew.Injection.ResolverItemsFactory.cs.generated.cs
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolverItemsFactory.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace OverallSuccess.SundewInjection
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class ResolverItemsFactory
    {
        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        internal static ResolverItem[] Create(int bucketSize, params ResolverItem[] resolvers)
        {
            var resolverItems = new ResolverItem[bucketSize];
            for (var i = 0; i < resolvers.Length; i++)
            {
                var resolverItem = resolvers[i];
                var index = RuntimeHelpers.GetHashCode(resolverItem.Type) % bucketSize;
                while (resolverItems[index].Type != default)
                {
                    index++;
                    if (index >= resolverItems.Length)
                    {
                        index = 0;
                    }
                }

                if (index >= resolverItems.Length)
                {
                    throw new NotSupportedException("The dictionary is full.");
                }

                resolverItems[index] = resolverItem;
            }

            return resolverItems;
        }
    }
}
