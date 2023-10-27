// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeId.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System;
using System.Text;

internal readonly struct TypeId : IEquatable<TypeId>
{
    private const string TypeName = "TypeId {";
    private readonly TypeIdWithHash typeId;

    public TypeId(string id)
    {
        this.typeId = new TypeIdWithHash(id, id.GetHashCode());
    }

    public string Id => this.typeId.Id;

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
        return new StringBuilder(TypeName, TypeName.Length + this.Id.Length + 2).Append(this.Id).Append(' ').Append('}').ToString();
    }

    private readonly record struct TypeIdWithHash(string Id, int HashCode);
}