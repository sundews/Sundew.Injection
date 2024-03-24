﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeContainerCodeGenerationProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.TypeResolver.CodeGenerationStage;

using Microsoft.CodeAnalysis;
using Sundew.Injection.Generator.Stages.CodeGeneration;
using Sundew.Injection.Generator.Stages.CodeGeneration.Templates;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.Features.TypeResolver.ResolveGraphStage;

public static class TypeContainerCodeGenerationProvider
{
    internal static IncrementalValuesProvider<GeneratedCodeOutput>
        SetupTypeResolverCodeGeneration(this IncrementalValuesProvider<(ResolvedTypeResolverDefinition ResolvedTypeResolverDefinition, CompilationData CompilationData)> resolveTypeResolverDefinitions)
    {
        return resolveTypeResolverDefinitions
            .Select((pair, cancellationToken) =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (resolvedTypeResolverDefinition, compilationData) = pair;
            var generatedTypeResolver = TypeResolverSyntaxGenerator.Generate(resolvedTypeResolverDefinition, compilationData);
            var typeResolverSourceCode =
                ImplementationSourceCodeEmitter.Emit(resolvedTypeResolverDefinition.Accessibility, generatedTypeResolver, new Options(true));
            var generatedCodeOutput = new GeneratedCodeOutput(generatedTypeResolver.Type.FullName, typeResolverSourceCode);
            return generatedCodeOutput;
        });
    }
}