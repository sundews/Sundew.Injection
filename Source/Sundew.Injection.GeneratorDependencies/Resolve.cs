// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Resolve.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection
{
    using System;

    internal delegate object Resolve<in TFactory>(TFactory factory, Span<object> arguments);
}