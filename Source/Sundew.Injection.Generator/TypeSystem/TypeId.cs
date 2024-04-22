// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeId.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System;
using System.Text;

internal readonly struct TypeId(string id) : IEquatable<TypeId>
{
    private const string TypeName = "TypeId { ";
    private readonly TypeIdWithHash typeId = new(id, id.GetHashCode());

    public string Identifier => this.typeId.Id;

    public static implicit operator string(TypeId typeId)
    {
        return typeId.Identifier;
    }

    public static bool operator ==(TypeId left, TypeId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TypeId left, TypeId right)
    {
        return !left.Equals(right);
    }

    public override bool Equals(object? obj)
    {
        return obj is TypeId other && this.Equals(other);
    }

    public bool Equals(TypeId other)
    {
        return this.typeId.Equals(other.typeId);
    }

    public override int GetHashCode()
    {
        return this.typeId.HashCode;
    }

    public override string ToString()
    {
        return new StringBuilder(TypeName, TypeName.Length + this.Identifier.Length + 2).Append(this.Identifier).Append(' ').Append('}').ToString();
    }

    private readonly record struct TypeIdWithHash(string Id, int HashCode);
}