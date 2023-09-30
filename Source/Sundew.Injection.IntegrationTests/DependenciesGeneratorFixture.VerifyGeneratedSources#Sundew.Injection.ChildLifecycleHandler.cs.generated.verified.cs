//HintName: Sundew.Injection.ChildLifecycleHandler.cs.generated.cs
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildLifecycleHandler.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection
{
    using System.Threading.Tasks;
    using global::Disposal.Interfaces;
    using global::Initialization.Interfaces;
    using Sundew.Injection.Disposal;
    using Sundew.Injection.Initialization;

    internal sealed class ChildLifecycleHandler : ILifecycleHandler
    {
        private readonly InitializingList<object> sharedInitializingList;
        private readonly InitializingList<object> initializingList;
        private readonly DisposingList<object> disposingList;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public ChildLifecycleHandler(
            InitializingList<object> sharedInitializingList,
            bool initializeConcurrently,
            bool disposeConcurrently,
            IInitializationReporter? initializationReporter,
            IDisposalReporter? disposalReporter)
        {
            this.sharedInitializingList = sharedInitializingList;
            this.initializingList = new InitializingList<object>(initializeConcurrently, initializationReporter);
            this.disposingList = new DisposingList<object>(disposeConcurrently, disposalReporter);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void TryAdd(object constructed)
        {
            this.initializingList.TryAdd(constructed);
            this.disposingList.TryAdd(constructed);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Initialize()
        {
            this.sharedInitializingList.Initialize();
            this.initializingList.Initialize();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public async ValueTask InitializeAsync()
        {
            await this.sharedInitializingList.InitializeAsync().ConfigureAwait(false);
            await this.initializingList.InitializeAsync().ConfigureAwait(false);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose()
        {
            this.disposingList.Dispose();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public async ValueTask DisposeAsync()
        {
            await this.disposingList.DisposeAsync().ConfigureAwait(false);
        }
    }
}