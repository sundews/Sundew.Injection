﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parameter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;

using System;
using Sundew.Base.Equality;
using Sundew.Injection.Generator.TypeSystem;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

internal class Parameter : IEquatable<Parameter>
{
    public Parameter(Type type, string name, bool mustNameMatchForEquality)
    {
        this.Type = type;
        this.Name = name;
        this.MustNameMatchForEquality = mustNameMatchForEquality;
    }

    public Type Type { get; }

    public string Name { get; }

    public bool MustNameMatchForEquality { get; }

    public bool Equals(Parameter? other)
    {
        return Equality.Equals(this, other, parameter =>
        {
            var typeEquals = Equals(this.Type, parameter.Type);
            if (this.MustNameMatchForEquality || parameter.MustNameMatchForEquality)
            {
                return typeEquals && this.Name == parameter.Name;
            }

            return typeEquals;
        });
    }

    public override bool Equals(object? obj)
    {
        return Equality.Equals(this, obj);
    }

    public override int GetHashCode()
    {
        if (this.MustNameMatchForEquality)
        {
            return HashCode.Combine(this.Type, this.Name);
        }

        return this.Type.GetHashCode();
    }
}