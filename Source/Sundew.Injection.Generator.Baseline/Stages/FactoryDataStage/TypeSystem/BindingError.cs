// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingError.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;

using System.Collections.Generic;
using System.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

[DiscriminatedUnions.DiscriminatedUnion]
public abstract record BindingError
{
    [DiscriminatedUnions.CaseType(typeof(CreateMethodError))]
    public static BindingError CreateMethodError(Type? unresolvedContainingType, ImmutableArray<Parameter> unresolvedParameters) => new CreateMethodError(unresolvedContainingType, unresolvedParameters);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(CreateGenericMethodError))]
    public static BindingError CreateGenericMethodError(ImmutableArray<(GenericParameter TargetParameter, Symbol UnresolvedSymbol)> failedParameters) => new CreateGenericMethodError(failedParameters);

    public sealed record FailedResolveError(ImmutableArray<FailedResolve> FailedResolves) : BindingError
    {
        public FailedResolveError(FailedResolve failedResolve)
            : this(ImmutableArray.Create(failedResolve))
        {
        }
    }

    public sealed record ResolveDefiniteParameterError(DefiniteParameter DefiniteParameter) : BindingError;

    public sealed record NoViableConstructorFoundForType(Type Type) : BindingError;

    public sealed record ResolveArrayElementsError(IReadOnlyList<BindingError> ElementsBindingError) : BindingError;
}