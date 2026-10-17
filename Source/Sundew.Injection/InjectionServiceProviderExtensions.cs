// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionServiceProviderExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Extends <see cref="IInjectionServiceProvider"/> with generic and throwing resolution methods.
/// </summary>
public static class InjectionServiceProviderExtensions
{
    /// <summary>
    /// Tries to get a service of the specified type by passing the specified arguments to the factory method creating it.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="service">The service.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns><c>true</c>, if the service could be resolved, otherwise <c>false</c>.</returns>
    public static bool TryGetService<TService>(this IInjectionServiceProvider serviceProvider, [MaybeNullWhen(false)] out TService service, params ReadOnlySpan<object?> arguments)
    {
        if (serviceProvider.TryGetService(typeof(TService), out var result, arguments) && result is TService typedService)
        {
            service = typedService;
            return true;
        }

        service = default;
        return false;
    }

    /// <summary>
    /// Gets a service of the specified type by passing the specified arguments to the factory method creating it.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>The service or <c>null</c>, if it could not be resolved.</returns>
    public static object? GetService(this IInjectionServiceProvider serviceProvider, Type serviceType, params ReadOnlySpan<object?> arguments)
    {
        return serviceProvider.TryGetService(serviceType, out var service, arguments) ? service : null;
    }

    /// <summary>
    /// Gets a service of the specified type by passing the specified arguments to the factory method creating it.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>The service.</returns>
    /// <exception cref="ServiceNotFoundException">Thrown when the service could not be resolved.</exception>
    public static object GetRequiredService(this IInjectionServiceProvider serviceProvider, Type serviceType, params ReadOnlySpan<object?> arguments)
    {
        if (serviceProvider.TryGetService(serviceType, out var service, arguments) && service != null)
        {
            return service;
        }

        throw new ServiceNotFoundException(serviceType, arguments.Length > 0);
    }

    /// <summary>
    /// Gets a service of the specified type by passing the specified arguments to the factory method creating it.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>The service.</returns>
    /// <exception cref="ServiceNotFoundException">Thrown when the service could not be resolved.</exception>
    public static TService GetRequiredService<TService>(this IInjectionServiceProvider serviceProvider, params ReadOnlySpan<object?> arguments)
    {
        return (TService)serviceProvider.GetRequiredService(typeof(TService), arguments);
    }
}
