// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndirectFactoryTargetAttribute.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection
{
    using System;

    /// <summary>
    /// Indicates that the decorated method is a factory target that does not return the created value directly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class IndirectFactoryTargetAttribute : Attribute
    {
    }
}