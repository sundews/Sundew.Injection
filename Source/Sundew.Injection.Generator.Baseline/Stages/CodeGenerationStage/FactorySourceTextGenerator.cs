// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactorySourceTextGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage;

using System.Collections.Immutable;
using System.Threading;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Templates;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage;

internal class FactorySourceTextGenerator
{
    public ImmutableArray<GeneratedOutput> CreateFactory(
        FactoryData factoryData,
        CompilationData compilationData,
        KnownSyntax knownSyntax,
        CancellationToken cancellationToken)
    {
        var factoryDeclarations = new FactorySyntaxGenerator(compilationData, knownSyntax, cancellationToken).Generate(factoryData);

        var options = new Options(compilationData.AreNullableAnnotationsSupported);
        var classText = FactoryImplementationFileGenerator.GetFileContent(Accessibility.Public, factoryDeclarations.ClassNamespaceDeclaration, options);
        var generatedOutputs = ImmutableArray.Create(new GeneratedOutput(factoryData.FactoryType.Name, classText));
        if (factoryData.GenerateInterface && factoryData.FactoryInterfaceType != null && factoryDeclarations.InterfaceNamespaceDeclaration != null)
        {
            var interfaceText = FactoryInterfaceFileTemplate.GetFileContent(Accessibility.Public, factoryDeclarations.InterfaceNamespaceDeclaration, options);
            generatedOutputs = generatedOutputs.Add(new GeneratedOutput(factoryData.FactoryInterfaceType.Name, interfaceText));
        }

        return generatedOutputs;
    }
}