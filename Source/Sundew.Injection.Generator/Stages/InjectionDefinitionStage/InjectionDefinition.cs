// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionDefinition.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal record InjectionDefinition(string DefaultNamespace,
    Inject RequiredParameterInjection,
    ValueArray<FactoryCreationDefinition> FactoryDefinitions,
    ValueDictionary<Type, ValueArray<BindingRegistration>> BindingRegistrations,
    ValueDictionary<UnboundGenericType, ValueArray<GenericBindingRegistration>> GenericBindingRegistrations,
    ValueDictionary<Type, ValueArray<ParameterSource>> RequiredParameters);
