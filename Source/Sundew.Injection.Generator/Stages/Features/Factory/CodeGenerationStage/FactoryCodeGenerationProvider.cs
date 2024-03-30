// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryCodeGenerationProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage;

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGeneration;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.CodeGeneration.Templates;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;

internal static class FactoryCodeGenerationProvider
{
    public static IncrementalValuesProvider<R<GeneratedOutput, ValueList<Diagnostic>>> SetupFactoryCodeGenerationStage(this IncrementalValuesProvider<(FactoryResolvedGraph FactoryData, CompilationData CompilationData)> factoryDataInput)
    {
        return factoryDataInput.Select((valueProvider, cancellationToken) => GetGeneratedOutput(valueProvider.FactoryData, valueProvider.CompilationData, cancellationToken));
    }

    internal static R<GeneratedOutput, ValueList<Diagnostic>> GetGeneratedOutput(FactoryResolvedGraph factoryResolvedGraph, CompilationData compilationData, CancellationToken cancellationToken)
    {
        try
        {
            var knownSyntax = new KnownSyntax(compilationData);
            var factoryDeclarations = new FactorySyntaxGenerator(compilationData, knownSyntax, factoryResolvedGraph, cancellationToken).Generate();

            var options = new Options(compilationData.AreNullableAnnotationsSupported);
            var classText = ImplementationSourceCodeEmitter.Emit(Sundew.Injection.Accessibility.Public, factoryDeclarations.ClassNamespaceDeclaration, options);
            var generatedOutputs = ImmutableArray.Create(new GeneratedCodeOutput(factoryResolvedGraph.FactoryType.FullName, classText));
            if (factoryResolvedGraph.FactoryInterfaceType != null && factoryDeclarations.InterfaceNamespaceDeclaration != null)
            {
                var interfaceText = InterfaceSourceCodeEmitter.Emit(Sundew.Injection.Accessibility.Public, factoryDeclarations.InterfaceNamespaceDeclaration, options);
                generatedOutputs = generatedOutputs.Add(new GeneratedCodeOutput(factoryResolvedGraph.FactoryInterfaceType.FullName, interfaceText));
            }

            return R.Success(new GeneratedOutput(new GeneratedTypeDeclaration(factoryResolvedGraph.FactoryType, factoryResolvedGraph.FactoryInterfaceType, factoryDeclarations.CreateMethods), generatedOutputs.ToValueArray()));
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