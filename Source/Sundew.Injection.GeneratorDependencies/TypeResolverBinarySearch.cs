// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeResolverBinarySearch.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class TypeResolverBinarySearch<TFactory>
    {
        private static readonly ResolverTypeComparer ResolverComparer = new ResolverTypeComparer();
        private readonly Resolver<TFactory>[] resolvers;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        internal TypeResolverBinarySearch(params Resolver<TFactory>[] resolvers)
        {
            this.resolvers = resolvers.OrderBy(x => x.Type.GetHashCode()).ToArray();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public object? Resolve(TFactory factory, Type type, Span<object> arguments)
        {
            var index = Array.BinarySearch(this.resolvers, new Resolver<TFactory>(type, default), ResolverComparer);

            if (index > -1)
            {
                return this.resolvers[index].Resolve?.Invoke(factory, arguments) ?? null;
            }

            return null;
        }

        internal sealed class ResolverTypeComparer : IComparer<Resolver<TFactory>>
        {
            public int Compare(Resolver<TFactory> x, Resolver<TFactory> y)
            {
                return x.Type.GetHashCode().CompareTo(y.Type.GetHashCode());
            }
        }
    }
}
