// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeContainerResolvedGraphProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.TypeResolver.ResolveGraphStage;

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CodeGeneration;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

public static class TypeContainerResolvedGraphProvider
{
    internal static IncrementalValuesProvider<ValueArray<R<ResolvedTypeResolverDefinition, Diagnostics>>>
        SetupResolveTypeResolverStage(
            this IncrementalValuesProvider<(ValueArray<ResolverCreationDefinition> ResolverCreationDefinitions,
                ImmutableArray<GeneratedTypeDeclaration> GeneratedFactoryDeclarations, CompilationData CompilationData)> typeResolverInputsProvider)
    {
        return typeResolverInputsProvider.Select((tuple, token) =>
        {
            var nameRegistry = new NameRegistry<(DefiniteType DefiniteType, ValueArray<DefiniteFactoryMethod> CreateMethod)>();
            var (resolverCreationDefinitions, generatedFactoryDeclarations, compilationData) = tuple;
            foreach (var generatedFactoryDeclaration in generatedFactoryDeclarations)
            {
                nameRegistry.Register(generatedFactoryDeclaration.ImplementationType.Name, (generatedFactoryDeclaration.ImplementationType, generatedFactoryDeclaration.CreateMethods));
                if (generatedFactoryDeclaration.InterfaceType.TryGetValue(out var factoryInterfaceType))
                {
                    nameRegistry.Register(factoryInterfaceType.Name, (factoryInterfaceType, generatedFactoryDeclaration.CreateMethods));
                }
            }

            token.ThrowIfCancellationRequested();
            var factoryTypeAndCreateMethodsResolver = new FactoryTypeAndCreateMethodsResolver(nameRegistry);
            var resolveCreationDefinitionResults = resolverCreationDefinitions
                .Select(resolverCreationDefinition => (resolverCreationDefinition,
                    createMethods: resolverCreationDefinition.FactoryRegistrations
                        .AllOrFailed(resolverCreationDefinitionResult => factoryTypeAndCreateMethodsResolver.ResolveFactoryRegistration(resolverCreationDefinitionResult).ToItem())));

            return resolveCreationDefinitionResults.Select(resolvedCreationDefinition =>
            {
                var (resolverCreationDefinition, createMethods) = resolvedCreationDefinition;
                if (createMethods.TryGet(out var all, out var failed))
                {
                    return R.Success(new ResolvedTypeResolverDefinition(
                        resolverCreationDefinition.ResolverType,
                        true,
                        all.Select(x => new ResolvedFactoryRegistration(x.DefiniteType, x.CreateMethods))
                            .ToValueArray(),
                        resolverCreationDefinition.Accessibility)).Omits<Diagnostics>();
                }

                return R.Error(new Diagnostics(failed.GetErrors().Select(GetDiagnostic)));
            }).ToValueArray();
        });
    }

    private static Diagnostic GetDiagnostic(FailedResolve failedResolve)
    {
        return Diagnostic.Create(Diagnostics.ResolveTypeError, Location.None, GetResolveTypeError(failedResolve));
    }

    private static string GetResolveTypeError(FailedResolve failedResolve)
    {
        static void AppendInner(FailedResolve failedResolve, int indentation, StringBuilder stringBuilder)
        {
            stringBuilder.If(indentation != 0, builder => builder.Append(' ', indentation)).AppendLine(failedResolve.Type.Name);
            var newIndentation = indentation + 2;
            if (failedResolve.InnerFailures.TryGetValue(out var inner))
            {
                foreach (var failedResolveInnerFailure in inner)
                {
                    AppendInner(failedResolveInnerFailure, newIndentation, stringBuilder);
                }
            }
        }

        var stringBuilder = new StringBuilder();
        AppendInner(failedResolve, 0, stringBuilder);
        return stringBuilder.ToString();
    }
}