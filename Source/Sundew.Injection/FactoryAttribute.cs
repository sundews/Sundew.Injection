// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryAttribute.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection;

using System;

/// <summary>
/// Indicates that the decorated class is a factory.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class FactoryAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FactoryAttribute"/> class.
    /// </summary>
    /// <param name="bindableFactoryTargets">The bindable factory targets.</param>
    public FactoryAttribute(params string[] bindableFactoryTargets)
    {
        this.BindableFactoryTargets = bindableFactoryTargets;
    }

    /// <summary>
    /// Gets the bindable factory targets.
    /// </summary>
    public string[] BindableFactoryTargets { get; }
}