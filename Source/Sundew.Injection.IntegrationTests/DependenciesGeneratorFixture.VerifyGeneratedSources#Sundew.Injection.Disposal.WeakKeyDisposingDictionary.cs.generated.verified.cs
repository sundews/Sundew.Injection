//HintName: Sundew.Injection.Disposal.WeakKeyDisposingDictionary.cs.generated.cs
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakKeyDisposingDictionary.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection.Disposal
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Disposal.Interfaces;
    using Sundew.Injection.WeakReferencing;

    internal class WeakKeyDisposingDictionary<TKey> : IDisposable, IAsyncDisposable
        where TKey : class
    {
        private readonly TimeSpan scheduleCleanThreshold;
        private readonly IDisposalReporter? disposalReporter;
        private IImmutableList<Item> disposables = ImmutableList<Item>.Empty;
        private DateTime lastScheduledClean;
        private int isCleaningScheduled;

        /// <summary>Initializes a new instance of the <see cref="WeakKeyDisposingDictionary{TKey}"/> class.</summary>
        public WeakKeyDisposingDictionary()
            : this(null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WeakKeyDisposingDictionary{TKey}"/> class.</summary>
        public WeakKeyDisposingDictionary(TimeSpan scheduleCleanThreshold)
            : this(scheduleCleanThreshold, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WeakKeyDisposingDictionary{TKey}"/> class.</summary>
        /// <param name="disposableReporter">The disposer reporter.</param>
        public WeakKeyDisposingDictionary(IDisposalReporter? disposableReporter)
            : this(TimeSpan.FromSeconds(5), disposableReporter)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WeakKeyDisposingDictionary{TKey}"/> class.</summary>
        /// <param name="scheduleCleanThreshold">The schedule clean threshold.</param>
        /// <param name="disposableReporter">The disposer reporter.</param>
        public WeakKeyDisposingDictionary(TimeSpan scheduleCleanThreshold, IDisposalReporter? disposableReporter)
        {
            this.scheduleCleanThreshold = scheduleCleanThreshold;
            this.lastScheduledClean = DateTime.MinValue;
            this.disposalReporter = disposableReporter;
        }

        /// <summary>
        /// Tries to add the specified object, if it is an disposable.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="disposable">The disposable.</param>
        /// <returns>A value indicating whether the object was added.</returns>
        public bool TryAdd(TKey key, object disposable)
        {
            var disposer = Disposer.TryGet(disposable);
            if (disposer != null)
            {
                this.PrivateAdd(key, disposer);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clears the disposables.
        /// </summary>
        public void Clear()
        {
            this.ReplaceList(disposables => disposables.Clear());
        }

        /// <summary>
        /// Disposes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Dispose(TKey key)
        {
            var (index, item) = this.FindAndRemove(this.disposables, key);
            if (index > -1)
            {
                this.ReplaceList(disposables => disposables.RemoveAt(index));
                foreach (var disposer in GetDisposers(item, this.ScheduleClean))
                {
                    disposer.Dispose(this.disposalReporter);
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            var disposables = this.disposables;
            this.Clear();
            foreach (var disposer in GetDisposers(disposables, this.ScheduleClean))
            {
                disposer.Dispose(this.disposalReporter);
            }
        }

        /// <summary>
        /// Disposes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>An async task.</returns>
        public async ValueTask DisposeAsync(TKey key)
        {
            var (index, item) = this.FindAndRemove(this.disposables, key);
            if (index > -1)
            {
                this.ReplaceList(disposables => disposables.RemoveAt(index));
                foreach (var disposer in GetDisposers(item, this.ScheduleClean))
                {
                    await disposer.DisposeAsync(this.disposalReporter).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <returns>An async task.</returns>
        public async ValueTask DisposeAsync()
        {
            var disposables = this.disposables;
            this.Clear();
            foreach (var disposer in GetDisposers(disposables, this.ScheduleClean))
            {
                await disposer.DisposeAsync(this.disposalReporter).ConfigureAwait(false);
            }
        }

        internal IEnumerable<Disposer> GetDisposers()
        {
            return GetDisposers(this.disposables, this.ScheduleClean);
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
                        var removedItems = this.disposables.Where(x => x.Key.TryGetTarget(out _)).ToArray();
                        this.ReplaceList(x => x.RemoveRange(removedItems));
                        Interlocked.Exchange(ref this.isCleaningScheduled, 0);
                    });
                }
            }
        }

        private static IEnumerable<Disposer> GetDisposers(IImmutableList<Item> disposers, Action scheduleCleanAction)
        {
            return disposers.SelectMany(x => GetDisposers(x, scheduleCleanAction));
        }

        private static IEnumerable<Disposer> GetDisposers(Item item, Action scheduleCleanAction)
        {
            yield return item.Disposer;
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

        private TActualKey PrivateAdd<TActualKey>(TActualKey key, Disposer disposer)
            where TActualKey : TKey
        {
            var item = new Item(key, disposer);
            this.ReplaceList(disposables => disposables.Insert(0, item));
            return key;
        }

        private void ReplaceList(Func<IImmutableList<Item>, IImmutableList<Item>> newListFunc)
        {
            var disposables = this.disposables;
            var newList = newListFunc(disposables);
            Interlocked.CompareExchange(ref this.disposables, newList, disposables);
            while (!ReferenceEquals(this.disposables, newList))
            {
                disposables = this.disposables;
                newList = newListFunc(disposables);
                Interlocked.CompareExchange(ref this.disposables, newList, disposables);
            }
        }

        private readonly struct Item
        {
            public Item(TKey key, Disposer disposer)
            {
                this.Key = new TargetEqualityWeakReference<TKey>(key);
                this.Disposer = disposer;
            }

            public TargetEqualityWeakReference<TKey> Key { get; }

            public Disposer Disposer { get; }
        }
    }
}