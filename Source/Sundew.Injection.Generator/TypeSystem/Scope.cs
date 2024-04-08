// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scope.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using Microsoft.CodeAnalysis;
using Sundew.DiscriminatedUnions;

[DiscriminatedUnion]
internal abstract partial record Scope
{
    public abstract Location Location { get; init; }

    internal sealed record Auto : Scope
    {
        public override Location Location { get; init; } = Location.None;
    }

    internal sealed record NewInstance(Location Location) : Scope;

    internal sealed record SingleInstancePerRequest(Location Location) : Scope;

    internal sealed record SingleInstancePerFuncResult(Method Method, Location Location) : Scope;

    internal sealed record SingleInstancePerFactory(Location Location) : Scope;
}
