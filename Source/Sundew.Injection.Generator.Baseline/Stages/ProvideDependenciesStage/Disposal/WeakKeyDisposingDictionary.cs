// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakKeyDisposingDictionary.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Disposal
{
    using System;

    internal class WeakKeyDisposingDictionary<TKey> : IDisposable
        where TKey : class
    {
        private ConcurrentWeakKeyDictionary<TKey, IDisposable>? disposables;

        public WeakKeyDisposingDictionary()
            : this(TimeSpan.FromMilliseconds(5000))
        {
        }

        public WeakKeyDisposingDictionary(TimeSpan scheduleCleanThreshold)
            : this(new ConcurrentWeakKeyDictionary<TKey, IDisposable>(scheduleCleanThreshold))
        {
        }

        internal WeakKeyDisposingDictionary(ConcurrentWeakKeyDictionary<TKey, IDisposable> disposables)
        {
            this.disposables = disposables;
            this.disposables.Cleaning += this.CleaningDisposables;
        }

        public bool TryAdd(TKey key, IDisposable disposable)
        {
            return this.disposables?.TryAdd(key, disposable) ?? false;
        }

        public void Dispose(TKey key)
        {
            if (this.disposables == null)
            {
                return;
            }

            if (this.disposables.TryRemove(key, out var disposable))
            {
                disposable?.Dispose();
            }
        }

        public void Dispose()
        {
            if (this.disposables != null)
            {
                foreach (var disposable in this.disposables)
                {
                    disposable.Value.Dispose();
                }
            }

            this.disposables = default;
        }

        internal bool TryAdd(TKey key, object @object)
        {
            if (@object is IDisposable disposable)
            {
                return this.TryAdd(key, disposable);
            }

            return false;
        }

        private void CleaningDisposables(object? sender, IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}