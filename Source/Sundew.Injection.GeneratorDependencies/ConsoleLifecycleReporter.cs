// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLifecycleReporter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection;

using System;
using global::Disposal.Interfaces;
using global::Initialization.Interfaces;

public sealed class ConsoleLifecycleReporter : IInitializationReporter, IDisposalReporter
{
    private const string Unknown = "<unknown>";

    public void Initialized(object source, IInitializable initializable)
    {
        Console.WriteLine($"Initialized: {initializable.GetType().FullName} by {GetName(source)}");
    }

    public void Initialized(object source, IAsyncInitializable initializable)
    {
        Console.WriteLine($"Initialized: {initializable.GetType().FullName} by {GetName(source)}");
    }

    public void Disposed(object source, IDisposable disposable)
    {
        Console.WriteLine($"Disposed: {disposable.GetType().FullName} by {GetName(source)}");
    }

    public void Disposed(object source, IAsyncDisposable disposable)
    {
        Console.WriteLine($"Disposed: {disposable.GetType().FullName} by {GetName(source)}");
    }

    private static string GetName(object source)
    {
        var type = source as Type ?? source.GetType();
        return type.FullName ?? source.ToString() ?? Unknown;
    }
}