// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultValueAttribute.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#nullable enable

namespace Sundew.Injection;

using System;

/// <summary>
/// Defines the default value for factory constructor parameters, as partial class methods does not support default values.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class DefaultValueAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultValueAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public DefaultValueAttribute(object? value)
    {
        this.Value = value;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public object? Value { get; }
}