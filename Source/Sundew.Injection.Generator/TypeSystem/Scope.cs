// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scope.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using Sundew.DiscriminatedUnions;

[DiscriminatedUnion]
internal abstract partial record Scope
{
    internal sealed record Auto : Scope;

    internal sealed record NewInstance : Scope;

    internal sealed record SingleInstancePerRequest : Scope;

    internal sealed record SingleInstancePerFuncResult(Method Method) : Scope;

    internal sealed record SingleInstancePerFactory : Scope;
}
