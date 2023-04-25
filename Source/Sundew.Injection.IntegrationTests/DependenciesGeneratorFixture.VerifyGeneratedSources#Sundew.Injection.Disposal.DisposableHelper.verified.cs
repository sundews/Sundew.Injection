//HintName: Sundew.Injection.Disposal.DisposableHelper.cs
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposableHelper.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Disposal
{
    using System;
    using System.Collections.Generic;

    internal static class DisposableHelper
    {
        public static void Dispose(object disposable)
        {
#if NETSTANDARD2_1
            DisposeItemPreferDispose(disposable).AsTask().Wait();
#else
            DisposeItemPreferDispose(disposable);
#endif
        }

        public static void Dispose(IEnumerable<object>? disposables)
        {
            if (disposables != null)
            {
                foreach (var disposable in disposables)
                {
#if NETSTANDARD2_1
                    DisposeItemPreferDispose(disposable).AsTask().Wait();
#else
                    DisposeItemPreferDispose(disposable);
#endif
                }
            }
        }

#if NETSTANDARD2_1
    public static async ValueTask DisposeAsync(object disposable, IDisposableReporter? disposableReporter)
    {
        await DisposeItemPreferAsyncDispose(disposable).ConfigureAwait(false);
    }

    public static async ValueTask DisposeAsync(IEnumerable<object>? disposables, IDisposableReporter? disposableReporter)
    {
        if (disposables != null)
        {
            if (disposableReporter != null)
            {
                foreach (var disposable in disposables)
                {
                    await DisposeAsyncAndReport(disposable, disposableReporter).ConfigureAwait(false);
                }
            }
            else
            {
                foreach (var item in disposables)
                {
                    await DisposeItemPreferAsyncDispose(item).ConfigureAwait(false);
                }
            }
        }
    }

    private static async Task DisposeAsyncAndReport(object disposable, IDisposableReporter disposableReporter)
    {
        if (disposable is IReportingDisposable reportingDisposable)
        {
            reportingDisposable.SetReporter(disposableReporter);
        }

        await DisposeItemPreferAsyncDispose(disposable).ConfigureAwait(false);
        disposableReporter.Disposed(disposable);
    }

    private static async Task DisposeItemPreferAsyncDispose(object item)
    {
        switch (item)
        {
            case IAsyncDisposable asyncDisposable:
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                break;
            case IDisposable disposable:
                disposable.Dispose();
                break;
        }
    }
#endif

#if NETSTANDARD2_1
    private static async ValueTask DisposeItemPreferDispose(object item)
    {
        switch (item)
        {
            case IDisposable disposable:
                disposable.Dispose();
                break;
            case IAsyncDisposable asyncDisposable:
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                break;
        }
    }
#else

        private static void DisposeItemPreferDispose(object item)
        {
            switch (item)
            {
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
#endif
    }
}