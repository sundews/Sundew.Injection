// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConcurrentWeakKeyDictionary.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Disposal
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;

    internal class ConcurrentWeakKeyDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : class
    {
        private readonly ConcurrentDictionary<TargetEqualityWeakReference<TKey>, TValue> dictionary = new ConcurrentDictionary<TargetEqualityWeakReference<TKey>, TValue>();
        private readonly TimeSpan scheduleCleanThreshold;
        private DateTime lastScheduledClean;
        private int isCleaningScheduled;

        public ConcurrentWeakKeyDictionary(TimeSpan scheduleCleanThreshold)
        {
            this.scheduleCleanThreshold = scheduleCleanThreshold;
            this.lastScheduledClean = DateTime.MinValue;
        }

        public event EventHandler<TValue>? Cleaning;

        public bool TryAdd(TKey key, TValue value)
        {
            var addResult = this.dictionary.TryAdd(new TargetEqualityWeakReference<TKey>(key), value);
            this.ScheduleClean();
            return addResult;
        }

        public bool TryRemove(TKey key, out TValue? value)
        {
            var removeResult = this.dictionary.TryRemove(new TargetEqualityWeakReference<TKey>(key), out value);
            this.ScheduleClean();
            return removeResult;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var scheduleClean = false;
            foreach (var value in this.dictionary)
            {
                if (value.Key.TryGetTarget(out var target))
                {
                    yield return new KeyValuePair<TKey, TValue>(target, value.Value);
                }
                else
                {
                    scheduleClean = this.isCleaningScheduled == 1;
                }
            }

            if (scheduleClean)
            {
                this.ScheduleClean();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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
                        foreach (var pair in this.dictionary)
                        {
                            if (!pair.Key.TryGetTarget(out var _))
                            {
                                this.dictionary.TryRemove(pair.Key, out var _);
                                this.Cleaning?.Invoke(this, pair.Value);
                            }
                        }

                        Interlocked.Exchange(ref this.isCleaningScheduled, 0);
                    });
                }
            }
        }
    }
}