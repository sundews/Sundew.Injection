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
    private readonly ImmutableArray<FactoryMethodRegistration>.Builder registrations = ImmutableArray.CreateBuilder<FactoryMethodRegistration>();

    public FactoryMethodRegistrationBuilder Add(
        FullType interfaceType,
        FullType implementationType,
        ScopeContext scope,
        Method method,
        string? factoryMethodName,
        Accessibility accessibility,
        bool isNewOverridable)
    {
        this.registrations.Add(new FactoryMethodRegistration(interfaceType, implementationType, scope, method, accessibility, isNewOverridable, factoryMethodName));
        return this;
    }

    public ValueArray<FactoryMethodRegistration> Build() => this.registrations.ToImmutable();
}