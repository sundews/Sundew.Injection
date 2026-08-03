// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethodRegistrationBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using System.Collections.Immutable;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal sealed class FactoryMethodRegistrationBuilder
{
    private readonly ImmutableDictionary<NamedType, ImmutableArray<FactoryMethodRegistration>>.Builder registrations = ImmutableDictionary.CreateBuilder<NamedType, ImmutableArray<FactoryMethodRegistration>>();

    public FactoryMethodRegistrationBuilder Add(
        NamedType containingType,
        ImmutableArray<FactoryMethodRegistration> factoryMethodRegistrations)
    {
        this.registrations.Add(containingType, factoryMethodRegistrations);
        return this;
    }

    public ValueDictionary<NamedType, ValueArray<FactoryMethodRegistration>> Build() => this.registrations.ToImmutableDictionary(x => x.Key, x => x.Value.ToValueArray());
}