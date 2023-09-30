// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

internal sealed record ErrorType(string Name) : Type(Name, false)
{
    public override TypeId Id => new("ErrorType: {this.Name}");
}