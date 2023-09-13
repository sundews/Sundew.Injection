// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInitializationParameters.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection;

using Initialization.Interfaces;

public interface IInitializationParameters
{
    bool InitializeConcurrently { get; }

    IInitializationReporter? InitializationReporter { get; }
}