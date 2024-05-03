// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializingList.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection.Dependencies.Initialization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Initialization.Interfaces;

    /// <summary>
    /// Stores <see cref="IInitializable"/> or <see cref="IAsyncInitializable"/> in a list for later initialization.
    /// </summary>
    /// <typeparam name="TInitializable">The type of the initializable.</typeparam>
    /// <seealso cref="System.IDisposable" />
    internal sealed class InitializingList<TInitializable> : IInitializable, IAsyncInitializable
    {
        private static readonly Task<bool> CompletedTrueTask = Task.FromResult(true);
        private readonly bool initializeConcurrently;
        private readonly IInitializationReporter? initializationReporter;
        private IImmutableList<Initializer> initializers = ImmutableList<Initializer>.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializingList{TItem}" /> class.
        /// </summary>
        public InitializingList()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializingList{TItem}" /> class.
        /// </summary>
        /// <param name="initializeConcurrently">if set to <c>true</c>  disposal will be executed concurrently.</param>
        /// <param name="initializationReporter">The initialization reporter.</param>
        public InitializingList(bool initializeConcurrently, IInitializationReporter? initializationReporter = null)
        {
            this.initializeConcurrently = initializeConcurrently;
            this.initializationReporter = initializationReporter;
        }

        /// <summary>
        /// Tries to add the specified object, if it is an initializable.
        /// </summary>
        /// <param name="object">The object.</param>
        /// <returns>A value indicating whether the object was added.</returns>
        public TObject TryAdd<TObject>(TObject @object)
        {
            var initializer = Initializer.TryGet(@object);
            if (initializer != null)
            {
                this.PrivateAdd(initializer);
            }

            return @object;
        }

        /// <summary>
        /// Initializes the specific item.
        /// </summary>
        /// <typeparam name="TActualInitializable">The actual type of the initializable.</typeparam>
        /// <param name="initializable">The initializable.</param>
        public void Initialize<TActualInitializable>(TActualInitializable initializable)
            where TActualInitializable : TInitializable, IInitializable
        {
            this.PrivateRemove(new Initializer.Synchronous(initializable));
            initializable.Initialize();
            this.initializationReporter?.Initialized(this.GetType(), initializable);
        }

        /// <summary>
        /// Initializes the specific item asynchronously.
        /// </summary>
        /// <typeparam name="TActualInitializable">The actual type of the initializable.</typeparam>
        /// <param name="asyncInitializable">The async disposable.</param>
        /// <returns>An async task.</returns>
        public async ValueTask InitializeAsync<TActualInitializable>(TActualInitializable asyncInitializable)
            where TActualInitializable : TInitializable, IAsyncInitializable
        {
            this.PrivateRemove(new Initializer.Asynchronous(asyncInitializable));
            await asyncInitializable.InitializeAsync();
            this.initializationReporter?.Initialized(this.GetType(), asyncInitializable);
        }

        /// <summary>
        /// Initializes the list.
        /// </summary>
        public void Initialize()
        {
            var initializers = this.initializers;
            this.Clear();
            if (this.initializeConcurrently)
            {
                Parallel.ForEach(initializers, x => x.Initialize(this.initializationReporter));
            }
            else
            {
                foreach (var disposer in initializers)
                {
                    disposer.Initialize(this.initializationReporter);
                }
            }
        }

        /// <summary>
        /// Initializes the list asynchronously.
        /// </summary>
        /// <returns>An async task.</returns>
        public async ValueTask InitializeAsync()
        {
            var initializers = this.initializers;
            this.Clear();
            if (this.initializeConcurrently)
            {
                await Task.WhenAll(initializers.Select(x =>
                {
                    var valueTask = x.InitializeAsync(this.initializationReporter);
                    if (valueTask.IsCompleted)
                    {
                        return CompletedTrueTask;
                    }

                    return valueTask.AsTask();
                })).ConfigureAwait(false);
            }
            else
            {
                foreach (var initializer in initializers)
                {
                    await initializer.InitializeAsync(this.initializationReporter).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Clear the initializing list.
        /// </summary>
        public void Clear()
        {
            this.ReplaceList(initializers => initializers.Clear());
        }

        internal IEnumerable<Initializer> GetInitializers()
        {
            return this.initializers;
        }

        private void PrivateAdd(Initializer initializer)
        {
            this.ReplaceList(initializers => initializers.Add(initializer));
        }

        private void PrivateRemove(Initializer initializer)
        {
            this.ReplaceList(initializers => initializers.Remove(initializer));
        }

        private void ReplaceList(Func<IImmutableList<Initializer>, IImmutableList<Initializer>> newListFunc)
        {
            var initializers = this.initializers;
            var newList = newListFunc(initializers);
            Interlocked.CompareExchange(ref this.initializers, newList, initializers);
            while (!ReferenceEquals(this.initializers, newList))
            {
                initializers = this.initializers;
                newList = newListFunc(initializers);
                Interlocked.CompareExchange(ref this.initializers, newList, initializers);
            }
        }
    }
}