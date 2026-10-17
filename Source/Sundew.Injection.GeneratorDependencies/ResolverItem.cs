// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolverItem.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection.Dependencies
{
    using System;

    internal readonly struct ResolverItem
    {
        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public ResolverItem(Type type, Resolve? resolve, params ArgumentResolver[] argumentResolvers)
        {
            this.Type = type;
            this.Resolve = resolve;
            this.TryDispose = null;
            this.TryDisposeAsync = null;
            this.ArgumentResolvers = argumentResolvers;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public ResolverItem(Type type, Resolve? resolve, TryDisposeInstance tryDispose, TryDisposeInstanceAsync tryDisposeAsync, params ArgumentResolver[] argumentResolvers)
        {
            this.Type = type;
            this.Resolve = resolve;
            this.TryDispose = tryDispose;
            this.TryDisposeAsync = tryDisposeAsync;
            this.ArgumentResolvers = argumentResolvers;
        }

        public Type Type { get; }

        public Resolve? Resolve { get; }

        public TryDisposeInstance? TryDispose { get; }

        public TryDisposeInstanceAsync? TryDisposeAsync { get; }

        public ArgumentResolver[] ArgumentResolvers { get; }
    }
}
