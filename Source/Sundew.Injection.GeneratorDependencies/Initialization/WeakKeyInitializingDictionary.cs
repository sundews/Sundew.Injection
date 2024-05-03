// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakKeyInitializingDictionary.cs" company="Sundews">
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
    using global::Sundew.Injection.Dependencies.WeakReferencing;

    internal class WeakKeyInitializingDictionary<TKey> : IInitializable, IAsyncInitializable
        where TKey : class
    {
        private readonly TimeSpan scheduleCleanThreshold;
        private readonly IInitializationReporter? initializationReporter;
        private IImmutableList<Item> initializables = ImmutableList<Item>.Empty;
        private DateTime lastScheduledClean;
        private int isCleaningScheduled;

        /// <summary>Initializes a new instance of the <see cref="WeakKeyInitializingDictionary{TKey}"/> class.</summary>
        public WeakKeyInitializingDictionary()
            : this(TimeSpan.FromSeconds(5), null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WeakKeyInitializingDictionary{TKey}"/> class.</summary>
        public WeakKeyInitializingDictionary(TimeSpan scheduleCleanThreshold)
            : this(scheduleCleanThreshold, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WeakKeyInitializingDictionary{TKey}"/> class.</summary>
        /// <param name="initializationReporter">The initialization reporter.</param>
        public WeakKeyInitializingDictionary(IInitializationReporter? initializationReporter)
            : this(TimeSpan.FromSeconds(5), initializationReporter)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WeakKeyInitializingDictionary{TKey}"/> class.</summary>
        /// <param name="scheduleCleanThreshold">The schedule clean threshold.</param>
        /// <param name="initializationReporter">The initialization reporter.</param>
        public WeakKeyInitializingDictionary(TimeSpan scheduleCleanThreshold, IInitializationReporter? initializationReporter)
        {
            this.scheduleCleanThreshold = scheduleCleanThreshold;
            this.lastScheduledClean = DateTime.MinValue;
            this.initializationReporter = initializationReporter;
        }

        /// <summary>
        /// Tries to add the specified object, if it is an initializable.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="initializable">The initializable.</param>
        /// <returns>A value indicating whether the object was added.</returns>
        public bool TryAdd(TKey key, object initializable)
        {
            var initializer = Initializer.TryGet(initializable);
            if (initializer != null)
            {
                this.PrivateAdd(key, initializer);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clears the initializables.
        /// </summary>
        public void Clear()
        {
            this.ReplaceList(initializables => initializables.Clear());
        }

        /// <summary>
        /// Initializes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Initialize(TKey key)
        {
            var (index, item) = this.FindAndRemove(this.initializables, key);
            if (index > -1)
            {
                this.ReplaceList(initializables => initializables.RemoveAt(index));
                foreach (var disposer in GetInitializers(item, this.ScheduleClean))
                {
                    disposer.Initialize(this.initializationReporter);
                }
            }
        }

        /// <summary>
        /// Initializes all items.
        /// </summary>
        public void Initialize()
        {
            var disposables = this.initializables;
            this.Clear();
            foreach (var disposer in GetInitializers(disposables, this.ScheduleClean))
            {
                disposer.Initialize(this.initializationReporter);
            }
        }

        /// <summary>
        /// Initializes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>An async task.</returns>
        public async ValueTask InitializeAsync(TKey key)
        {
            var (index, item) = this.FindAndRemove(this.initializables, key);
            if (index > -1)
            {
                this.ReplaceList(disposables => disposables.RemoveAt(index));
                foreach (var disposer in GetInitializers(item, this.ScheduleClean))
                {
                    await disposer.InitializeAsync(this.initializationReporter).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Initializes all items asynchronously.
        /// </summary>
        /// <returns>An async task.</returns>
        public async ValueTask InitializeAsync()
        {
            var disposables = this.initializables;
            this.Clear();
            foreach (var initializer in GetInitializers(disposables, this.ScheduleClean))
            {
                await initializer.InitializeAsync(this.initializationReporter).ConfigureAwait(false);
            }
        }

        internal IEnumerable<Initializer> GetInitializers()
        {
            return GetInitializers(this.initializables, this.ScheduleClean);
        }

        internal void ScheduleClean()
        {
            var utcNow = DateTime.UtcNow;
            if (utcNow - this.lastScheduledClean > this.scheduleCleanThreshold)
            {
                this.lastScheduledClean = utcNow;
                if (Interlocked.Exchange(ref this.isCleaningScheduled, 1) == 0)
                {
                    new CleanOperation(() =>
                    {
                        var removedItems = this.initializables.Where(x => x.Key.TryGetTarget(out _)).ToArray();
                        this.ReplaceList(x => x.RemoveRange(removedItems));
                        Interlocked.Exchange(ref this.isCleaningScheduled, 0);
                    });
                }
            }
        }

        private static IEnumerable<Initializer> GetInitializers(IImmutableList<Item> initializers, Action scheduleCleanAction)
        {
            return initializers.SelectMany(x => GetInitializers(x, scheduleCleanAction));
        }

        private static IEnumerable<Initializer> GetInitializers(Item item, Action scheduleCleanAction)
        {
            yield return item.Initilizer;
            if (!item.Key.TryGetTarget(out _))
            {
                scheduleCleanAction();
            }
        }

        private (int Index, Item Item) FindAndRemove(IImmutableList<Item> immutableList, TKey key)
        {
            for (var index = 0; index < immutableList.Count; index++)
            {
                var item = immutableList[index];
                if (Equals(item.Key, key))
                {
                    return (index, item);
                }
            }

            return (-1, default);
        }

        private TActualKey PrivateAdd<TActualKey>(TActualKey key, Initializer initializer)
            where TActualKey : TKey
        {
            var item = new Item(key, initializer);
            this.ReplaceList(initilizables => initilizables.Add(item));
            return key;
        }

        private void ReplaceList(Func<IImmutableList<Item>, IImmutableList<Item>> newListFunc)
        {
            var initializables = this.initializables;
            var newList = newListFunc(initializables);
            Interlocked.CompareExchange(ref this.initializables, newList, initializables);
            while (!ReferenceEquals(this.initializables, newList))
            {
                initializables = this.initializables;
                newList = newListFunc(initializables);
                Interlocked.CompareExchange(ref this.initializables, newList, initializables);
            }
        }

        private readonly struct Item
        {
            public Item(TKey key, Initializer initializer)
            {
                this.Key = new TargetEqualityWeakReference<TKey>(key);
                this.Initilizer = initializer;
            }

            public TargetEqualityWeakReference<TKey> Key { get; }

            public Initializer Initilizer { get; }
        }
    }
}