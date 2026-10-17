// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceNotFoundException.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection;

using System;

/// <summary>
/// Exception thrown when a required service could not be resolved from an <see cref="IInjectionServiceProvider"/>.
/// </summary>
public sealed class ServiceNotFoundException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNotFoundException"/> class.
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <param name="hasArguments">A value indicating whether arguments were specified.</param>
    public ServiceNotFoundException(Type serviceType, bool hasArguments)
        : base(hasArguments
            ? $"The service type: {serviceType} could not be resolved with the specified arguments."
            : $"The service type: {serviceType} could not be resolved.")
    {
        this.ServiceType = serviceType;
    }

    /// <summary>
    /// Gets the service type.
    /// </summary>
    public Type ServiceType { get; }
}
