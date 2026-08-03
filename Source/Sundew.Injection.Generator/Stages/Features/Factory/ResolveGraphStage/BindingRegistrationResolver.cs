// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingRegistrationResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal class BindingRegistrationResolver
{
    private readonly ValueDictionary<TypeId, ValueArray<BindingRegistration>> bindingRegistrations;
    private readonly ValueDictionary<TypeId, ValueArray<BindingRegistration>> fallbackBindingRegistrations;

    public BindingRegistrationResolver(
        ValueDictionary<TypeId, ValueArray<BindingRegistration>> bindingRegistrations,
        ValueDictionary<TypeId, ValueArray<BindingRegistration>> fallbackBindingRegistrations)
    {
        this.bindingRegistrations = bindingRegistrations;
        this.fallbackBindingRegistrations = fallbackBindingRegistrations;
    }

    public bool TryGetValue(TypeId typeId, out ValueArray<BindingRegistration> foundRegistrations)
    {
        if (this.bindingRegistrations.TryGetValue(typeId, out foundRegistrations))
        {
            return true;
        }

        if (this.fallbackBindingRegistrations.TryGetValue(typeId, out foundRegistrations))
        {
            return true;
        }

        foundRegistrations = ValueArray<BindingRegistration>.Empty;
        return false;
    }
}