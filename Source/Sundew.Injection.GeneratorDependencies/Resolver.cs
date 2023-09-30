// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Resolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection
{
    using System;

    internal readonly struct Resolver<TFactory>
    {
        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public Resolver(Type type, Resolve<TFactory>? resolve)
        {
            this.Type = type;
            this.Resolve = resolve;
        }

        public Type Type { get; }

        public Resolve<TFactory>? Resolve { get; }
    }
}