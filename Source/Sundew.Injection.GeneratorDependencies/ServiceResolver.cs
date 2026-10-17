// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection.Dependencies
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal sealed class ServiceResolver
    {
        private readonly ResolverItem[] resolverItems;
        private readonly TryDisposeInstance[] instanceDisposers;
        private readonly TryDisposeInstanceAsync[] asyncInstanceDisposers;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public ServiceResolver(int bucketSize, params ResolverItem[] resolverItems)
        {
            this.resolverItems = ResolverItemsFactory.Create(bucketSize, resolverItems);
            var instanceDisposers = new List<TryDisposeInstance>();
            var asyncInstanceDisposers = new List<TryDisposeInstanceAsync>();
            foreach (var resolverItem in resolverItems)
            {
                if (resolverItem.TryDispose != null && resolverItem.TryDisposeAsync != null)
                {
                    instanceDisposers.Add(resolverItem.TryDispose);
                    asyncInstanceDisposers.Add(resolverItem.TryDisposeAsync);
                }
            }

            this.instanceDisposers = instanceDisposers.ToArray();
            this.asyncInstanceDisposers = asyncInstanceDisposers.ToArray();
        }

        public bool TryGetService(Type serviceType, out object? service, ReadOnlySpan<object?> arguments)
        {
            if (this.TryFind(serviceType, out var index))
            {
                ref readonly var resolverItem = ref this.resolverItems[index];
                if (arguments.Length == 0)
                {
                    var resolve = resolverItem.Resolve;
                    if (resolve != null)
                    {
                        service = resolve.Invoke();
                        return service != null;
                    }
                }
                else if (TryGetArgumentResolver(resolverItem.ArgumentResolvers, arguments, out var resolveWithArguments))
                {
                    service = resolveWithArguments.Invoke(arguments);
                    return service != null;
                }
            }

            service = null;
            return false;
        }

        public bool TryDispose(object instance)
        {
            var wasDisposed = false;
            foreach (var instanceDisposer in this.instanceDisposers)
            {
                wasDisposed |= instanceDisposer.Invoke(instance);
            }

            return wasDisposed;
        }

        public async ValueTask<bool> TryDisposeAsync(object instance)
        {
            var wasDisposed = false;
            foreach (var asyncInstanceDisposer in this.asyncInstanceDisposers)
            {
                wasDisposed |= await asyncInstanceDisposer.Invoke(instance).ConfigureAwait(false);
            }

            return wasDisposed;
        }

        private static bool TryGetArgumentResolver(ArgumentResolver[] argumentResolvers, ReadOnlySpan<object?> arguments, out ResolveWithArguments resolveWithArguments)
        {
            foreach (var argumentResolver in argumentResolvers)
            {
                if (argumentResolver.Accepts(arguments, true))
                {
                    resolveWithArguments = argumentResolver.Resolve;
                    return true;
                }
            }

            foreach (var argumentResolver in argumentResolvers)
            {
                if (argumentResolver.Accepts(arguments, false))
                {
                    resolveWithArguments = argumentResolver.Resolve;
                    return true;
                }
            }

            resolveWithArguments = default!;
            return false;
        }

        private bool TryFind(Type serviceType, out int index)
        {
            var length = this.resolverItems.Length;
            index = ResolverItemsFactory.GetStartIndex(serviceType, length);
            for (var i = 0; i < length; i++)
            {
                var type = this.resolverItems[index].Type;
                if (type == null)
                {
                    return false;
                }

                if (ReferenceEquals(type, serviceType))
                {
                    return true;
                }

                index++;
                if (index == length)
                {
                    index = 0;
                }
            }

            return false;
        }
    }
}
