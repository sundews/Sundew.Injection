// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionDefinition.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal record InjectionDefinition(
    Inject RequiredParameterInjection,
    ValueArray<FactoryCreationDefinition> FactoryCreationDefinitions,
    ValueDictionary<TypeId, ValueArray<BindingRegistration>> BindingRegistrations,
    ValueDictionary<UnboundGenericType, ValueArray<GenericBindingRegistration>> GenericBindingRegistrations,
    ValueDictionary<TypeId, ValueArray<ParameterSource>> RequiredParameterSources,
    ValueDictionary<TypeId, ScopeContext> RequiredParameterScopes,
    ValueArray<ResolverCreationDefinition> ResolverCreationDefinitions);
