// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionStageError.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract record InjectionStageError
{
    public sealed record ResolveTypeError(BindingError BindingError, string ParentNode) : InjectionStageError;

    public sealed record ResolveParameterError(Type Type, string ParentNode, ValueArray<ParameterSource> ParameterSources) : InjectionStageError;

    public sealed record ScopeError(DefiniteType DefiniteType, Scope Scope, string ParentNode, string ParentScope) : InjectionStageError;
}