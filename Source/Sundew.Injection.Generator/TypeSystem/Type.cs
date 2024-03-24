// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Kind.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

[DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record Type(string Name, bool IsValueType) : Symbol(Name)
{
    public abstract TypeId Id { get; }

    public override string ToString()
    {
        return this.Id.ToString();
    }
}