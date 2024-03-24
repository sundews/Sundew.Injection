// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionStageError.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record InjectionStageError
{
    public sealed record UnsupportedInstanceMethod(DefiniteMethod Method, DefiniteType Type, string DependeeNodeName) : InjectionStageError;

    public sealed record ResolveTypeError(BindingError BindingError, string DependeeNodeName) : InjectionStageError;

    public sealed record ResolveParameterError(Type Type, string DependeeNodeName, ValueArray<ParameterSource> ParameterSources) : InjectionStageError;

    public sealed record ScopeError(DefiniteType DefiniteType, Scope Scope, string DependeeNodeName, string DependeeScope) : InjectionStageError;
}