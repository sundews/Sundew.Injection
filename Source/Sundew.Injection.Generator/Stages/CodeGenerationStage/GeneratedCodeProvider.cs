// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratedCodeProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage;

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Primitives.Computation;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Templates;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage;

internal static class GeneratedCodeProvider
{
    public static IncrementalValuesProvider<R<ValueArray<GeneratedOutput>, ValueList<Diagnostic>>> SetupCodeGenerationStage(IncrementalValuesProvider<(FactoryData FactoryData, CompilationData CompilationData)> factoryDataInput)
    {
        return factoryDataInput.Select((valueProvider, cancellationToken) => GetGeneratedOutput(valueProvider.FactoryData, valueProvider.CompilationData, cancellationToken));
    }

    internal static R<ValueArray<GeneratedOutput>, ValueList<Diagnostic>> GetGeneratedOutput(FactoryData factoryData, CompilationData compilationData, CancellationToken cancellationToken)
    {
        try
        {
            var knownSyntax = new KnownSyntax(compilationData);
            var factoryDeclarations = new FactorySyntaxGenerator(compilationData, knownSyntax, factoryData, cancellationToken).Generate();

            var options = new Options(compilationData.AreNullableAnnotationsSupported);
            var classText = FactoryImplementationSourceCodeEmitter.GetFileContent(Sundew.Injection.Accessibility.Public, factoryDeclarations.ClassNamespaceDeclaration, options);
            var generatedOutputs = ImmutableArray.Create(new GeneratedOutput(factoryData.FactoryType.Name, classText));
            if (factoryData.GenerateInterface && factoryData.FactoryInterfaceType != null && factoryDeclarations.InterfaceNamespaceDeclaration != null)
            {
                var interfaceText = FactoryInterfaceSourceCodeEmitter.GetFileContent(Sundew.Injection.Accessibility.Public, factoryDeclarations.InterfaceNamespaceDeclaration, options);
                generatedOutputs = generatedOutputs.Add(new GeneratedOutput(factoryData.FactoryInterfaceType.Name, interfaceText));
            }

            return R.Success(generatedOutputs.ToValueArray());
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return R.Error(ImmutableArray.Create(Diagnostic.Create(Diagnostics.UnknownError, default, e.ToString()))
                .ToValueList());
        }
    }
}