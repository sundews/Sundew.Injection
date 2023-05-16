//HintName: Sundew.Injection.LifecycleHandler.cs
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LifecycleHandler.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection
{
    using System.Threading.Tasks;
    using global::Disposal.Interfaces;
    using global::Initialization.Interfaces;
    using Sundew.Injection.Disposal;
    using Sundew.Injection.Initialization;

    internal sealed class LifecycleHandler : ILifecycleHandler
    {
        private readonly bool parallelize;
        private readonly IInitializationReporter? initializationReporter;
        private readonly IDisposalReporter? disposalReporter;
        private readonly InitializingList<object> sharedInitializingList;
        private readonly DisposingList<object> sharedDisposingList;

        private readonly WeakKeyInitializingDictionary<object> perRequestInitializingDictionary;

        private readonly WeakKeyDisposingDictionary<object> perRequestDisposingDictionary;

        public LifecycleHandler(bool parallelize, IInitializationReporter? initializationReporter, IDisposalReporter? disposalReporter)
        {
            this.parallelize = parallelize;
            this.initializationReporter = initializationReporter;
            this.disposalReporter = disposalReporter;
            this.sharedInitializingList = new InitializingList<object>(parallelize, initializationReporter);
            this.sharedDisposingList = new DisposingList<object>(parallelize, disposalReporter);
            this.perRequestInitializingDictionary =
                new WeakKeyInitializingDictionary<object>(initializationReporter);
            this.perRequestDisposingDictionary = new WeakKeyDisposingDictionary<object>(disposalReporter);
        }

        public void TryAdd(object constructed)
        {
            this.sharedInitializingList.TryAdd(constructed);
            this.sharedDisposingList.TryAdd(constructed);
        }

        public void TryAdd(object constructed, object target)
        {
            this.perRequestInitializingDictionary.TryAdd(constructed, target);
            this.perRequestDisposingDictionary.TryAdd(constructed, target);
        }

        public void Initialize()
        {
            this.sharedInitializingList.Initialize();
            this.perRequestInitializingDictionary.Initialize();
        }

        public async ValueTask InitializeAsync()
        {
            await this.sharedInitializingList.InitializeAsync().ConfigureAwait(false);
            await this.perRequestInitializingDictionary.InitializeAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            this.perRequestDisposingDictionary.Dispose();
            this.sharedDisposingList.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await this.perRequestDisposingDictionary.DisposeAsync().ConfigureAwait(false);
            await this.sharedDisposingList.DisposeAsync().ConfigureAwait(false);
        }

        public void Dispose(object constructed)
        {
            this.perRequestDisposingDictionary.Dispose(constructed);
        }

        public ValueTask DisposeAsync(object constructed)
        {
            return this.perRequestDisposingDictionary.DisposeAsync(constructed);
        }

        public ILifecycleHandler CreateChildLifecycleHandler()
        {
            return new ChildLifecycleHandler(this.sharedInitializingList, this.parallelize, this.initializationReporter, this.disposalReporter);
        }

        internal sealed class ChildLifecycleHandler : ILifecycleHandler
        {
            private readonly InitializingList<object> sharedInitializingList;
            private readonly InitializingList<object> initializingList;
            private readonly DisposingList<object> disposingList;

            public ChildLifecycleHandler(
                InitializingList<object> sharedInitializingList,
                bool parallelize,
                IInitializationReporter? initializationReporter,
                IDisposalReporter? disposalReporter)
            {
                this.sharedInitializingList = sharedInitializingList;
                this.initializingList = new InitializingList<object>(parallelize, initializationReporter);
                this.disposingList = new DisposingList<object>(parallelize, disposalReporter);
            }

            public void TryAdd(object constructed)
            {
                this.initializingList.TryAdd(constructed);
                this.disposingList.TryAdd(constructed);
            }

            public void Initialize()
            {
                this.sharedInitializingList.Initialize();
                this.initializingList.Initialize();
            }

            public async ValueTask InitializeAsync()
            {
                await this.sharedInitializingList.InitializeAsync().ConfigureAwait(false);
                await this.initializingList.InitializeAsync().ConfigureAwait(false);
            }

            public void Dispose()
            {
                this.disposingList.Dispose();
            }

            public async ValueTask DisposeAsync()
            {
                await this.disposingList.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}