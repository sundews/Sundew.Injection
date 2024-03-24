// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Kind.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;

using System.Collections.Generic;
using System.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

[DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record BindingError
{
    internal sealed record FailedResolveError(ImmutableArray<FailedResolve> FailedResolves) : BindingError
    {
        public FailedResolveError(FailedResolve failedResolve)
            : this(ImmutableArray.Create(failedResolve))
        {
        }
    }
}

internal sealed record ResolveDefiniteParameterError(DefiniteParameter DefiniteParameter) : BindingError;

internal sealed record NoViableConstructorFoundForType(Type Type) : BindingError;

internal sealed record ResolveArrayElementsError(IReadOnlyList<BindingError> ElementsBindingError) : BindingError;

internal sealed record CreateMethodError(Type? UnresolvedContainingType, ImmutableArray<Parameter> UnresolvedParameters) : BindingError;

internal sealed record CreateGenericMethodError(ImmutableArray<(GenericParameter TargetParameter, Symbol UnresolvedSymbol)> FailedParameters) : BindingError;