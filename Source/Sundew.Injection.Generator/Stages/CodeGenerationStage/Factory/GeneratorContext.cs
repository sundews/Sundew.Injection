// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratorContext.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System.Threading;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage;

internal sealed record GeneratorContext(FactoryData FactoryData, CompilationData CompilationData,
    KnownSyntax KnownSyntax, CancellationToken CancellationToken);
