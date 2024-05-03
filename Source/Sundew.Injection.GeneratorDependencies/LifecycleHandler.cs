// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LifecycleHandler.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection.Dependencies
{
    using System.Threading.Tasks;
    using global::Disposal.Interfaces;
    using global::Initialization.Interfaces;
    using global::Sundew.Injection;
    using global::Sundew.Injection.Dependencies.Disposal;
    using global::Sundew.Injection.Dependencies.Initialization;

    internal sealed class LifecycleHandler : ILifecycleHandler
    {
        private readonly bool initializeConcurrently;
        private readonly bool disposeConcurrently;
        private readonly IInitializationReporter? initializationReporter;
        private readonly IDisposalReporter? disposalReporter;
        private readonly InitializingList<object> sharedInitializingList;
        private readonly DisposingList<object> sharedDisposingList;

        private readonly WeakKeyInitializingDictionary<object> perRequestInitializingDictionary;

        private readonly WeakKeyDisposingDictionary<object> perRequestDisposingDictionary;

        public LifecycleHandler(IInitializationParameters? initializationParameters = null, IDisposalParameters? disposalParameters = null)
            : this(initializationParameters?.InitializeConcurrently ?? false, disposalParameters?.DisposeConcurrently ?? false, initializationParameters?.InitializationReporter, disposalParameters?.DisposalReporter)
        {
        }

        public LifecycleHandler(bool initializeConcurrently, bool disposeConcurrently, IInitializationReporter? initializationReporter, IDisposalReporter? disposalReporter)
        {
            this.initializeConcurrently = initializeConcurrently;
            this.disposeConcurrently = disposeConcurrently;
            this.initializationReporter = initializationReporter;
            this.disposalReporter = disposalReporter;
            this.sharedInitializingList = new InitializingList<object>(initializeConcurrently, initializationReporter);
            this.sharedDisposingList = new DisposingList<object>(disposeConcurrently, disposalReporter);
            this.perRequestInitializingDictionary =
                new WeakKeyInitializingDictionary<object>(initializationReporter);
            this.perRequestDisposingDictionary = new WeakKeyDisposingDictionary<object>(disposalReporter);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public TConstructed TryAdd<TConstructed>(TConstructed constructed)
        {
            this.sharedInitializingList.TryAdd(constructed);
            this.sharedDisposingList.TryAdd(constructed);
            return constructed;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void TryAdd(object constructed, object target)
        {
            this.perRequestInitializingDictionary.TryAdd(constructed, target);
            this.perRequestDisposingDictionary.TryAdd(constructed, target);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Initialize()
        {
            this.sharedInitializingList.Initialize();
            this.perRequestInitializingDictionary.Initialize();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public async ValueTask InitializeAsync()
        {
            await this.sharedInitializingList.InitializeAsync().ConfigureAwait(false);
            await this.perRequestInitializingDictionary.InitializeAsync().ConfigureAwait(false);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose()
        {
            this.perRequestDisposingDictionary.Dispose();
            this.sharedDisposingList.Dispose();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public async ValueTask DisposeAsync()
        {
            await this.perRequestDisposingDictionary.DisposeAsync().ConfigureAwait(false);
            await this.sharedDisposingList.DisposeAsync().ConfigureAwait(false);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose(object constructed)
        {
            this.perRequestDisposingDictionary.Dispose(constructed);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public ValueTask DisposeAsync(object constructed)
        {
            return this.perRequestDisposingDictionary.DisposeAsync(constructed);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public ChildLifecycleHandler CreateChildLifecycleHandler()
        {
            return new ChildLifecycleHandler(this.sharedInitializingList, this.initializeConcurrently, this.disposeConcurrently, this.initializationReporter, this.disposalReporter);
        }
    }
}