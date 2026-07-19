// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSource.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using Sundew.Injection.Generator.TypeSystem;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record ParameterSource(Type Type, string Name, bool IsMember, bool NeedsInvocation, bool IsOptional);

internal sealed record DirectParameter(Type Type, string Name, bool IsMember, bool NeedsInvocation, ParameterNecessity ParameterNecessity, Inject Inject) : ParameterSource(Type, Name, IsMember, NeedsInvocation, ParameterNecessity.IsOptional)
{
    public override string ToString()
    {
        return $"Direct: Name: {this.Name}, IsOptional: {this.IsOptional}, ParameterNecessity: {this.ParameterNecessity.ToString()}";
    }
}

internal sealed record PropertyAccessorParameter(Type Type, string Name, bool IsMember, AccessorProperty AccessorProperty, bool NeedsInvocation) : ParameterSource(Type, Name, IsMember, NeedsInvocation, AccessorProperty.IsParameterOptional | AccessorProperty.IsResultOptional)
{
    public override string ToString()
    {
        return $"Property: {this.AccessorProperty.ContainingType.FullName}.{this.AccessorProperty.Name}, IsParameterOptional: {this.AccessorProperty.IsParameterOptional}, IsResultOptional: {this.AccessorProperty.IsResultOptional}";
    }
}