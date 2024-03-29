﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratorContext.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage;

using System.Threading;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;

internal sealed record GeneratorContext(
    FactoryResolvedGraph FactoryResolvedGraph,
    CompilationData CompilationData,
    KnownSyntax KnownSyntax,
    CancellationToken CancellationToken);
