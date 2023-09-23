// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection
{
    using System;
    using System.Linq;

    internal delegate object Resolve<in TFactory>(TFactory factory, Span<object> arguments);

    public sealed class TypeResolver<TFactory>
    {
        private readonly Resolver[] resolvers;

        internal TypeResolver(params Resolver[] resolvers)
        {
            this.resolvers = resolvers;
        }

        public object Resolve(TFactory factory, Type type, Span<object> arguments)
        {
            var resolver = this.resolvers.FirstOrDefault(x => x.Type == type);
            if (!resolver.Equals(default))
            {
                return resolver.Resolve(factory, arguments);
            }

            throw new NotSupportedException($"Resolving type: {type} failed");
        }

        internal readonly record struct Resolver(Type Type, Resolve<TFactory> Resolve);
    }
}