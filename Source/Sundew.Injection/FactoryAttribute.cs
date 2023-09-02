﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryAttribute.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection;

using System;

/// <summary>
/// Indicates that the decorated class is a factory.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class FactoryAttribute : Attribute
{
}