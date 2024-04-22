// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolverRegistrationBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using System.Collections.Immutable;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal sealed class FactoryRegistrationBuilder
{
    private readonly ImmutableArray<FactoryRegistration>.Builder registrations = ImmutableArray.CreateBuilder<FactoryRegistration>();

    public FactoryRegistrationBuilder Add(Type factoryType, ValueArray<FactoryTarget> factoryMethods)
    {
        this.registrations.Add(new FactoryRegistration(factoryType, factoryMethods));
        return this;
    }

    public ValueArray<FactoryRegistration> Build() => this.registrations.ToImmutable();
}