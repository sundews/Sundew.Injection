// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionStageError.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record InjectionStageError
{
    public sealed record UnsupportedInstanceMethodError(Method Method, Type Type, string DependantNodeName) : InjectionStageError;

    public sealed record CreateGenericMethodError(TypeSystem.CreateGenericMethodError Error, string DependantNodeName) : InjectionStageError;

    public sealed record ResolveParameterError(Type Type, string DependantNodeName, ValueArray<ParameterSource> ParameterSources) : InjectionStageError;

    public sealed record ScopeError(Type Type, Scope Scope, string DependantNodeName, string DependantScope) : InjectionStageError;

    public sealed record ReferencedTypeMismatchError(Type ActualParameterType, Type ReferencedType, Scope Scope, string DependantNodeName) : InjectionStageError;
}