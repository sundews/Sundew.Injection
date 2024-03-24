// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryDefinitionResult.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;

using Microsoft.CodeAnalysis;
using Sundew.Base.Collections.Immutable;

internal abstract record FactoryDefinitionResult
{
    public sealed record Success(FactoryResolvedGraph FactoryResolvedGraph) : FactoryDefinitionResult;

    public sealed record Error(ValueList<Diagnostic> Diagnostics) : FactoryDefinitionResult;
}