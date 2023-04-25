// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputCodeGenerationProvider.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.OutputStage;

using Microsoft.CodeAnalysis;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGenerationStage;

internal static class OutputCodeGenerationProvider
{
    public static void SetupOutputResultStage(
        this IncrementalValuesProvider<ValueArray<GeneratedOutput>> generatedCodeProvider,
        IncrementalGeneratorInitializationContext incrementalGeneratorInitializationContext)
    {
        incrementalGeneratorInitializationContext.RegisterSourceOutput(generatedCodeProvider, (sourceProductionContext, generatedOutputs) =>
        {
            foreach (var generatedOutput in generatedOutputs)
            {
                sourceProductionContext.AddSource(generatedOutput.FileName, generatedOutput.Source);
            }
        });
    }
}