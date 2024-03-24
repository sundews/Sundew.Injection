// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndirectCreateMethodAttribute.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection;

using System;

/// <summary>
/// Indicates that the decorated method can act as a create method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class IndirectCreateMethodAttribute : Attribute
{
}