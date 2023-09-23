// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethodRegistrationBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;
using Scope = Sundew.Injection.Scope;

internal sealed class FactoryMethodRegistrationBuilder
{
    private readonly TypeFactory typeFactory;
    private readonly ImmutableArray<FactoryMethodRegistration>.Builder registrations = ImmutableArray.CreateBuilder<FactoryMethodRegistration>();

    public FactoryMethodRegistrationBuilder(TypeFactory typeFactory)
    {
        this.typeFactory = typeFactory;
    }

    public FactoryMethodRegistrationBuilder Add(ITypeSymbol interfaceTypeSymbol, ITypeSymbol implementationType, Method? method, string? createMethodName, Injection.Accessibility accessibility, bool isNewOverridable)
    {
        var defaultConstructor = TypeHelper.GetDefaultConstructorMethod(implementationType);
        var actualMethod = defaultConstructor != null ? this.typeFactory.CreateMethod(defaultConstructor) : new Method(ImmutableArray<Parameter>.Empty, implementationType.MetadataName, this.typeFactory.CreateType(implementationType).Type, true);

        var interfaceType = this.typeFactory.CreateType(interfaceTypeSymbol);
        this.registrations.Add(new FactoryMethodRegistration(interfaceType, this.typeFactory.CreateType(implementationType), Scope.NewInstance, actualMethod, accessibility, isNewOverridable, createMethodName));
        return this;
    }

    public ValueArray<FactoryMethodRegistration> Build() => this.registrations.ToImmutable();
}