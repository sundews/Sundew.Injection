// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjectionServiceProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection;

using System;
using System.Threading.Tasks;

/// <summary>
/// Interface for a generated service provider, which in addition to <see cref="System.IServiceProvider"/> supports non throwing resolution, resolution with arguments and disposal of resolved services.
/// </summary>
public interface IInjectionServiceProvider : System.IServiceProvider
{
    /// <summary>
    /// Tries to get a service of the specified type by passing the specified arguments to the factory method creating it.
    /// The factory method is selected by the number of arguments and their runtime types. A <c>null</c> argument matches any parameter accepting <c>null</c>.
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <param name="service">The service.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns><c>true</c>, if the service could be resolved, otherwise <c>false</c>.</returns>
    bool TryGetService(Type serviceType, out object? service, params ReadOnlySpan<object?> arguments);

    /// <summary>
    /// Tries to dispose the specified instance, by passing it to the dispose method of every factory registered for a service type that the instance is.
    /// A factory only disposes instances it has created, so the instance does not need to be passed as the type it was resolved as.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <returns><c>true</c>, if the instance is of a service type with a disposing factory, otherwise <c>false</c>.</returns>
    bool TryDispose(object instance);

    /// <summary>
    /// Tries to dispose the specified instance asynchronously, by passing it to the dispose method of every factory registered for a service type that the instance is.
    /// A factory only disposes instances it has created, so the instance does not need to be passed as the type it was resolved as.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <returns>An async task with a value indicating whether the instance is of a service type with a disposing factory.</returns>
    ValueTask<bool> TryDisposeAsync(object instance);
}
