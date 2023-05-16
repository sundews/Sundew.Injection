// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposingList.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Disposal
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Stores <see cref="IDisposable"/> in a list for later disposal.
    /// </summary>
    /// <typeparam name="TDisposable">The type of the disposable.</typeparam>
    /// <seealso cref="System.IDisposable" />
    internal sealed class DisposingList<TDisposable> : IDisposable
    {
        private readonly object lockObject = new object();
        private readonly List<object> disposables = new List<object>();

        /// <summary>
        /// Adds the specified disposable.
        /// </summary>
        /// <typeparam name="TActualDisposable">The type of the actual disposable.</typeparam>
        /// <param name="disposable">The disposable.</param>
        /// <returns>
        /// The added disposable.
        /// </returns>
        public TActualDisposable Add<TActualDisposable>(TActualDisposable disposable)
            where TActualDisposable : TDisposable, IDisposable
        {
            lock (this.lockObject)
            {
                this.disposables.Add(disposable);
            }

            return disposable;
        }

        /// <summary>
        /// Adds the specified disposable.
        /// </summary>
        /// <param name="disposable">The disposable.</param>
        public void Add(IDisposable disposable)
        {
            lock (this.lockObject)
            {
                this.disposables.Add(disposable);
            }
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        public void AddRange(IEnumerable<IDisposable> disposables)
        {
            lock (this.lockObject)
            {
                this.disposables.AddRange(disposables);
            }
        }

        /// <summary>Disposes the specified disposable.</summary>
        /// <typeparam name="TActualDisposable">The actual disposable type.</typeparam>
        /// <param name="disposable">The disposable.</param>
        public void Dispose<TActualDisposable>(TActualDisposable disposable)
            where TActualDisposable : TDisposable, IDisposable
        {
            lock (this.lockObject)
            {
                DisposableHelper.Dispose(disposable);
                this.disposables.Remove(disposable);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock (this.lockObject)
            {
                DisposableHelper.Dispose(this.disposables);
                this.disposables.Clear();
            }
        }

        /// <summary>
        /// Adds the specified disposable.
        /// </summary>
        /// <param name="object">The object.</param>
        internal void Add(object @object)
        {
            if (@object is IDisposable disposable)
            {
                this.Add(disposable);
            }
        }
    }
}