// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveWithArguments.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection.Dependencies
{
    using System;

    internal delegate object ResolveWithArguments(ReadOnlySpan<object?> arguments);
}
