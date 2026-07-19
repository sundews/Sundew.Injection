// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterNecessity.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System.Diagnostics.CodeAnalysis;

[DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record ParameterNecessity
{
    public abstract bool IsOptional { get; }

    internal sealed record Required : ParameterNecessity
    {
        public override bool IsOptional => false;
    }

    internal sealed record Optional(bool HasDefaultValue, object? DefaultValue) : ParameterNecessity
    {
        [MemberNotNullWhen(true, nameof(DefaultValue))]
        public bool HasDefaultValue { get; init; } = HasDefaultValue;

        public override bool IsOptional => true;
    }
}