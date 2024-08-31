// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeContainerResolvedGraphProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.TypeResolver.ResolveGraphStage;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGeneration;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

public static class TypeContainerResolvedGraphProvider
{
    internal static IncrementalValuesProvider<ValueArray<ResolvedTypeResolverDefinition>>
        SetupResolveTypeResolverStage(
            this IncrementalValuesProvider<(ValueArray<ServiceProviderImplementationDefinition> ResolverCreationDefinitions,
                ImmutableArray<GeneratedTypeDeclaration> GeneratedFactoryDeclarations, CompilationData CompilationData)> typeResolverInputsProvider)
    {
        return typeResolverInputsProvider.Select((tuple, token) =>
        {
            var nameRegistry = new NameRegistry<ValueArray<FactoryTargetDeclaration>>();
            var (resolverCreationDefinitions, generatedFactoryDeclarations, compilationData) = tuple;
            foreach (var generatedFactoryDeclaration in generatedFactoryDeclarations)
            {
                nameRegistry.Register(generatedFactoryDeclaration.ImplementationType.Name, generatedFactoryDeclaration.FactoryTargets);
                if (generatedFactoryDeclaration.InterfaceType.TryGetValue(out var factoryInterfaceType))
                {
                    nameRegistry.Register(factoryInterfaceType.Name, generatedFactoryDeclaration.FactoryTargets);
                }
            }

            token.ThrowIfCancellationRequested();
            var factoryTypeAndCreateMethodsResolver = new FactoryTypeAndCreateMethodsResolver(nameRegistry);
            IEnumerable<(ServiceProviderImplementationDefinition ResolverCreationDefinition, IEnumerable<(Type Type, ValueArray<FactoryTargetDeclaration> FactoryTargets)> FactoryTargets)> resolveCreationDefinitionResults = resolverCreationDefinitions
                .Select(resolverCreationDefinition => (resolverCreationDefinition,
                    createMethods: resolverCreationDefinition.FactoryRegistrations
                        .Select(resolverCreationDefinitionResult => factoryTypeAndCreateMethodsResolver.ResolveFactoryRegistration(resolverCreationDefinitionResult))));

            return resolveCreationDefinitionResults.Select(resolvedCreationDefinition =>
            {
                var (resolverCreationDefinition, factoryTargets) = resolvedCreationDefinition;
                return new ResolvedTypeResolverDefinition(
                          resolverCreationDefinition.ResolverType,
                          true,
                          factoryTargets.Select(x => new ResolvedFactoryRegistration(x.Type, x.FactoryTargets))
                              .ToValueArray(),
                          resolverCreationDefinition.Accessibility);
            }).ToValueArray();
        });
    }
}