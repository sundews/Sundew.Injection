// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILifecycleHandler.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection
{
    using System;
    using global::Initialization.Interfaces;

    /// <summary>
    /// Interface for handling the lifecycle of an object created by a factory.
    /// </summary>
    public interface ILifecycleHandler : IInitializable, IAsyncInitializable, IDisposable, IAsyncDisposable
    {
    }
}