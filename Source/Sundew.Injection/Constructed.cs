// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constructed.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection;

/// <summary>
/// Contains the result of an factory 'CreateUninitialized' call.
/// </summary>
/// <typeparam name="TObject">The object type.</typeparam>
public readonly struct Constructed<TObject>
        where TObject : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Constructed{TObject}"/> struct.
    /// </summary>
    /// <param name="object">The object.</param>
    /// <param name="lifecycleHandler">The lifecycle handler.</param>
    public Constructed(TObject @object, ILifecycleHandler lifecycleHandler)
    {
        this.Object = @object;
        this.LifecycleHandler = lifecycleHandler;
    }

    /// <summary>
    /// Gets the object.
    /// </summary>
    public TObject Object { get; }

    /// <summary>
    /// Gets the lifecycle handler.
    /// </summary>
    public ILifecycleHandler LifecycleHandler { get; }
}
