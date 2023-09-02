// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LifecycleParameters.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection;

using Disposal.Interfaces;
using Initialization.Interfaces;

public sealed class LifecycleParameters : ILifecycleParameters
{
    public LifecycleParameters(
        bool initializeConcurrently = false,
        bool disposeConcurrently = false,
        IInitializationReporter? initializationReporter = default,
        IDisposalReporter? disposalReporter = default)
    {
        this.InitializeConcurrently = initializeConcurrently;
        this.DisposeConcurrently = disposeConcurrently;
        this.InitializationReporter = initializationReporter;
        this.DisposalReporter = disposalReporter;
    }

    public static LifecycleParameters Default { get; } = new LifecycleParameters(false, false, null, null);

    public IInitializationParameters InitializationParameters => this;

    public IDisposalParameters DisposalParameters => this;

    public bool InitializeConcurrently { get; }

    public bool DisposeConcurrently { get; }

    public IInitializationReporter? InitializationReporter { get; }

    public IDisposalReporter? DisposalReporter { get; }

    public static LifecycleParameters Create<TLifecycleReporter>(TLifecycleReporter lifecycleReporter)
        where TLifecycleReporter : IInitializationReporter, IDisposalReporter
    {
        return new LifecycleParameters(false, false, lifecycleReporter, lifecycleReporter);
    }
}