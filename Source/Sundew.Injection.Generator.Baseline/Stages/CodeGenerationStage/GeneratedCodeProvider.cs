// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratedCodeProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage;

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.FactoryDataStage;

internal class GeneratedCodeProvider(
    FactorySourceTextGenerator factorySourceTextGenerator)
{
    public IncrementalValuesProvider<R<ValueArray<GeneratedOutput>, ValueList<Diagnostic>>> SetupCodeGenerationStage(IncrementalValuesProvider<(FactoryData FactoryData, CompilationDataStage.CompilationData CompilationData)> factoryDataInput)
    {
        return factoryDataInput.Select((valueProvider, cancellationToken) =>
        {
            try
            {
                var factoryData = valueProvider.FactoryData;
                var compilationData = valueProvider.CompilationData;
                var knownSyntax = new KnownSyntax(compilationData);
                var generatedOutputs = factorySourceTextGenerator.CreateFactory(
                    factoryData,
                    compilationData,
                    knownSyntax,
                    cancellationToken);
                return (R<ValueArray<GeneratedOutput>, ValueList<Diagnostic>>)R.Success(generatedOutputs.ToValueArray());
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                return R.Error(ImmutableArray.Create(Diagnostic.Create(Diagnostics.UnknownError, default, e.ToString())).ToValueList());
            }
        });
    }
}