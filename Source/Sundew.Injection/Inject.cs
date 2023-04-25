// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Inject.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection
{
    /// <summary>
    /// Describes how parameters should be injected.
    /// </summary>
    public enum Inject
    {
        ByType,
        ByTypeAndName,
        Separately,
    }
}