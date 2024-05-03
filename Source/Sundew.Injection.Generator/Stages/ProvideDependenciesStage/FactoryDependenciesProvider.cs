// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryDependenciesProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.ProvideDependenciesStage;

using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

public static class FactoryDependenciesProvider
{
    public const string SundewInjectionDependenciesNamespace = "Sundew.Injection.Dependencies";

    public static void ProvideDependencies(this IncrementalGeneratorInitializationContext context)
    {
        var generatorDependenciesProvider = context.CompilationProvider.Select((compilation, _) =>
        {
            var interfaceResources = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(x => x.StartsWith(GetBaseNamespacePath())).ToValueArray();
            return new GeneratorDependencies(TypeHelper.GetAssemblySundewInjectionDependenciesNamespace(compilation), interfaceResources);
        });

        context.RegisterSourceOutput(generatorDependenciesProvider, async (context, dependenciesProvider) =>
        {
            var interfaceResources = dependenciesProvider.DependencyResourceNames;
            await interfaceResources.ForEachAsync(async x =>
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(x);
                if (stream is { })
                {
                    using StreamReader reader = new(stream);
                    var sourceText = await reader.ReadToEndAsync();
                    sourceText = sourceText.Replace(SundewInjectionDependenciesNamespace, dependenciesProvider.AssemblySundewInjectionNamespace);
                    context.AddSource($"{typeof(IInjectionDeclaration).Namespace}.{x.Substring(typeof(FactoryDependenciesProvider).Namespace.Length + 1)}.generated", sourceText);
                }
            });
        });
    }

    private static string GetBaseNamespacePath()
    {
        return typeof(FactoryDependenciesProvider).Namespace;
    }
}

internal record GeneratorDependencies(string AssemblySundewInjectionNamespace, ValueArray<string> DependencyResourceNames);