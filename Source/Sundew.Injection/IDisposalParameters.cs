// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDisposalParameters.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection
{
    using Disposal.Interfaces;

    public interface IDisposalParameters
    {
        bool DisposeConcurrently { get; }

        IDisposalReporter? DisposalReporter { get; }
    }
}