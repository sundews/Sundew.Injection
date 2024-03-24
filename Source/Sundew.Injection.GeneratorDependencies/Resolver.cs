// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Resolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    /*
    public class Resolver
    {
        public void T(IServiceCollection serviceCollection)
        {
        }
    }

    internal abstract class ResolverProvider : IServiceProvider
    {
        private readonly IResolverScope serviceScope;

        public ResolverProvider()
            : this(null)
        {
        }

        public ResolverProvider(IResolverScope? serviceScope)
        {
            this.serviceScope = serviceScope ?? new ResolverScope(this);
        }

        public object GetService(Type serviceType)
        {
            var index = global::System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(serviceType) % BucketSize;
            do
            {
                ref var item = ref this.resolverItems[index];
                if (ReferenceEquals(item.Type, serviceType))
                {
                    var result = item.Resolve.Invoke();
                    if (item.Dispose != null)
                    {
                        var disposeMethod = item.Dispose;
                        this.serviceScope.Add(() => disposeMethod.Invoke(result));
                    }
                }
            }
            while (index++ < BucketSize);

            throw new global::System.NotSupportedException($"The type: {serviceType} could not be found.");
        }
    }

    public class Factory : IServiceScopeFactory
    {
        private readonly IServiceProvider serviceProvider;

        public Factory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IServiceScope CreateScope()
        {
            return new ResolverScope(this.serviceProvider);
        }
    }

    public class ResolverScope : IResolverScope
    {
        private readonly List<Func<ValueTask>> owned = new List<Func<ValueTask>>();

        public ResolverScope(ResolverProvider resolverProvider)
        {
            this.ServiceProvider = new ResolverProvider(this);
        }

        public IServiceProvider ServiceProvider { get; }

        public void Add(Func<ValueTask> disposeAction)
        {
            this.owned.Add(disposeAction);
        }

        public void Dispose()
        {
            this.owned.ForEach(x => x.Invoke());
            this.owned.Clear();
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var action in this.owned)
            {
                await action.Invoke();
            }

            this.owned.Clear();
        }
    }

    internal interface IResolverScope : IServiceScope, IAsyncDisposable
    {
        void Add(Func<ValueTask> disposeAction);
    }*/
}