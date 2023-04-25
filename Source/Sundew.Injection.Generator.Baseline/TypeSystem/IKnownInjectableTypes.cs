// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IKnownInjectableTypes.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using Microsoft.CodeAnalysis;

public interface IKnownInjectableTypes
{
    INamedTypeSymbol IEnumerableTypeSymbol { get; }

    INamedTypeSymbol IDisposableTypeSymbol { get; }
}