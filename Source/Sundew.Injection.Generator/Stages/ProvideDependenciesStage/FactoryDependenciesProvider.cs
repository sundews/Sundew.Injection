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

public static class FactoryDependenciesProvider
{
    public static void ProvideDependencies(this IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(async context =>
        {
            var interfaceResources = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(x => x.StartsWith(GetBaseNamespacePath()));
            await interfaceResources.ForEachAsync(async x =>
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(x);
                using StreamReader reader = new(stream);
                context.AddSource($"{typeof(IInjectionDeclaration).Namespace}.{x.Substring(typeof(FactoryDependenciesProvider).Namespace.Length + 1)}.generated", await reader.ReadToEndAsync());
            });
        });
    }

    private static string GetBaseNamespacePath()
    {
        return typeof(FactoryDependenciesProvider).Namespace;
    }
}