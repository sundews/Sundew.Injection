// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Symbol.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

[DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record Symbol(string Name) : ISymbol
{
    public virtual string FullName => this.Name;
}