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

    public abstract Scope ToDependencyScope();

    internal sealed record Auto : Scope
    {
        public override Location Location { get; init; } = Location.None;

        public override Scope ToDependencyScope()
        {
            return _NewInstance(this.Location);
        }
    }

    internal sealed record NewInstance(Location Location) : Scope
    {
        public override Scope ToDependencyScope()
        {
            return _NewInstance(Location.None);
        }
    }

    internal sealed record SingleInstancePerRequest(Location Location) : Scope
    {
        public override Scope ToDependencyScope()
        {
            return _SingleInstancePerRequest(Location.None);
        }
    }

    internal sealed record SingleInstancePerFuncResult(Method Method, Location Location) : Scope
    {
        public override Scope ToDependencyScope()
        {
            return _SingleInstancePerRequest(Location.None);
        }
    }

    internal sealed record SingleInstancePerFactory(string? ExposeAsPropertyOption, Location Location) : Scope
    {
        public override Scope ToDependencyScope()
        {
            return _SingleInstancePerFactory(default, Location.None);
        }
    }
}
