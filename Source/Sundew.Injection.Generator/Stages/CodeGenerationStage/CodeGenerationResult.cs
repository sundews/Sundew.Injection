// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeGenerationResult.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage;

using Microsoft.CodeAnalysis;
using Sundew.Base.Collections.Immutable;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record CodeGenerationResult;

internal sealed record Success(ValueArray<GeneratedOutput> GeneratedOutputs) : CodeGenerationResult;

internal sealed record Error(ValueArray<Diagnostic> Diagnostics) : CodeGenerationResult;