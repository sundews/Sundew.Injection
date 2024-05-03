// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LifecycleParameters.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection;

using Disposal.Interfaces;
using Initialization.Interfaces;

public sealed class LifecycleParameters(bool initializeConcurrently = false,
        bool disposeConcurrently = false,
        IInitializationReporter? initializationReporter = default,
        IDisposalReporter? disposalReporter = default)
    : ILifecycleParameters
{
    public static LifecycleParameters Default { get; } = new();

    public IInitializationParameters InitializationParameters => this;

    public IDisposalParameters DisposalParameters => this;

    public bool InitializeConcurrently { get; } = initializeConcurrently;

    public bool DisposeConcurrently { get; } = disposeConcurrently;

    public IInitializationReporter? InitializationReporter { get; } = initializationReporter;

    public IDisposalReporter? DisposalReporter { get; } = disposalReporter;

    public static LifecycleParameters Create<TLifecycleReporter>(TLifecycleReporter lifecycleReporter)
        where TLifecycleReporter : IInitializationReporter, IDisposalReporter
    {
        return new LifecycleParameters(false, false, lifecycleReporter, lifecycleReporter);
    }
}