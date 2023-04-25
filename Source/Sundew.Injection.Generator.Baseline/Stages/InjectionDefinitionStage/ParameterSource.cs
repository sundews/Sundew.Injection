// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSource.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record ParameterSource;

internal sealed record DirectParameter(Inject Inject) : ParameterSource
{
    public override string ToString()
    {
        return $"Direct: {this.Inject}";
    }
}

internal sealed record PropertyAccessorParameter(AccessorProperty AccessorProperty) : ParameterSource
{
    public override string ToString()
    {
        return $"Property: {this.AccessorProperty.ContainingType.FullName}.{this.AccessorProperty.Name}";
    }
}