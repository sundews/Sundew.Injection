// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryDefinitionResult.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage;

using Microsoft.CodeAnalysis;
using Sundew.Base.Collections.Immutable;

internal abstract record FactoryDefinitionResult
{
    public sealed record Success(FactoryData FactoryData) : FactoryDefinitionResult;

    public sealed record Error(ValueList<Diagnostic> Diagnostics) : FactoryDefinitionResult;
}