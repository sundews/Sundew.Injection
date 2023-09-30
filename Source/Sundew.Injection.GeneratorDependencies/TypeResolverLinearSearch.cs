// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeResolverLinearSearch.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection
{
    using System;

    internal sealed class TypeResolverLinearSearch<TFactory>
    {
        private readonly Resolver<TFactory>[] resolvers;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        internal TypeResolverLinearSearch(params Resolver<TFactory>[] resolvers)
        {
            this.resolvers = resolvers;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public object? Resolve(TFactory factory, Type type, Span<object> arguments)
        {
            for (int i = 0; i < this.resolvers.Length; i++)
            {
                var resolver = this.resolvers[i];
                if (resolver.Type == type)
                {
                    return resolver.Resolve?.Invoke(factory, arguments);
                }
            }

            return null;
        }
    }
}
