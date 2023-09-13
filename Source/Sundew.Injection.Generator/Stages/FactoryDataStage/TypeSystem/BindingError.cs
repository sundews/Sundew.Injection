// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Kind.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;

using System.Collections.Generic;
using System.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

[DiscriminatedUnions.DiscriminatedUnion]
internal abstract record BindingError
{
    [DiscriminatedUnions.CaseType(typeof(CreateMethodError))]
    internal static BindingError CreateMethodError(Type? unresolvedContainingType, ImmutableArray<Parameter> unresolvedParameters) => new CreateMethodError(unresolvedContainingType, unresolvedParameters);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(CreateGenericMethodError))]
    internal static BindingError CreateGenericMethodError(ImmutableArray<(GenericParameter TargetParameter, Symbol UnresolvedSymbol)> failedParameters) => new CreateGenericMethodError(failedParameters);

    internal sealed record FailedResolveError(ImmutableArray<FailedResolve> FailedResolves) : BindingError
    {
        public FailedResolveError(FailedResolve failedResolve)
            : this(ImmutableArray.Create(failedResolve))
        {
        }
    }

    internal sealed record ResolveDefiniteParameterError(DefiniteParameter DefiniteParameter) : BindingError;

    internal sealed record NoViableConstructorFoundForType(Type Type) : BindingError;

    internal sealed record ResolveArrayElementsError(IReadOnlyList<BindingError> ElementsBindingError) : BindingError;
}