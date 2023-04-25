//HintName: Sundew.Injection.Disposal.Disposer.cs
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Disposer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Disposal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An implementation of <see cref="IDisposable"/> that disposes an list of <see cref="IDisposable"/>.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    internal sealed class Disposer : IDisposable
    {
        private IReadOnlyCollection<object>? disposables;

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposer"/> class.
        /// </summary>
        public Disposer()
            : this(Enumerable.Empty<IDisposable>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposer"/> class.
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        public Disposer(params IDisposable[] disposables)
            : this((IEnumerable<IDisposable>)disposables)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Disposer"/> class.</summary>
        /// <param name="disposables">The disposables.</param>
        public Disposer(IEnumerable<IDisposable> disposables)
        {
            this.disposables = disposables.ToList();
        }

#if NETSTANDARD2_1
    /// <summary>
    /// Initializes a new instance of the <see cref="Disposer"/> class.
    /// </summary>
    /// <param name="disposables">The disposables.</param>
    public Disposer(params IAsyncDisposable[] disposables)
        : this((IEnumerable<IAsyncDisposable>)disposables)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Disposer"/> class.</summary>
    /// <param name="disposableReporter">The disposable reporter.</param>
    /// <param name="disposables">The disposables.</param>
    public Disposer(IDisposableReporter disposableReporter, params IAsyncDisposable[] disposables)
        : this(disposables, disposableReporter)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Disposer"/> class.</summary>
    /// <param name="disposables">The disposables.</param>
    /// <param name="disposableReporter">The disposable reporter.</param>
    public Disposer(IEnumerable<IAsyncDisposable> disposables, IDisposableReporter? disposableReporter = null)
    {
        this.disposableReporter = disposableReporter;
        this.disposables = disposables.ToReadOnly();
        this.disposableReporter?.SetSource(this);
    }
#endif

        /// <summary>Initializes a new instance of the <see cref="Disposer"/> class.</summary>
        /// <param name="disposables">The disposables.</param>
        private Disposer(IEnumerable<object> disposables)
        {
            this.disposables = disposables.ToList();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            DisposableHelper.Dispose(this.disposables);
            this.disposables = null;
        }
    }
}