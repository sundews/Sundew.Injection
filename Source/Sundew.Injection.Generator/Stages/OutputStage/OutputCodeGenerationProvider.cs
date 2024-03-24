// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputCodeGenerationProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.OutputStage;

using Microsoft.CodeAnalysis;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGeneration;

internal static class OutputCodeGenerationProvider
{
    private const string Generated = ".generated";

    public static void SetupOutputResultStage(
        this IncrementalValuesProvider<ValueArray<GeneratedCodeOutput>> generatedCodeProvider,
        IncrementalGeneratorInitializationContext incrementalGeneratorInitializationContext)
    {
        incrementalGeneratorInitializationContext.RegisterSourceOutput(generatedCodeProvider, (sourceProductionContext, generatedOutputs) =>
        {
            foreach (var generatedOutput in generatedOutputs)
            {
                sourceProductionContext.AddSource(generatedOutput.FileName + Generated, generatedOutput.Source);
            }
        });
    }

    public static void SetupOutputResultStage(
        this IncrementalValuesProvider<GeneratedCodeOutput> generatedCodeProvider,
        IncrementalGeneratorInitializationContext incrementalGeneratorInitializationContext)
    {
        incrementalGeneratorInitializationContext.RegisterSourceOutput(generatedCodeProvider, (sourceProductionContext, generatedOutput) =>
        {
            sourceProductionContext.AddSource(generatedOutput.FileName + Generated, generatedOutput.Source);
        });
    }
}