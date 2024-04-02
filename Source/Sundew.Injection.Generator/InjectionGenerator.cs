// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator;

using Microsoft.CodeAnalysis;
using Sundew.Base.Collections;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;
using Sundew.Injection.Generator.Stages.Features.TypeResolver.CodeGenerationStage;
using Sundew.Injection.Generator.Stages.Features.TypeResolver.ResolveGraphStage;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.Stages.OutputStage;
using Sundew.Injection.Generator.Stages.SemanticModelStage;

[Generator]
public class InjectionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var accessibleConstructorProvider = context.SyntaxProvider.SetupAccessibleConstructorStage();

        var (successCompilationDataProvider, errorCompilationInfoProvider) = context.CompilationProvider.SetupCompilationDataStage().SegregateByResult();

        context.RegisterSourceOutput(errorCompilationInfoProvider, (productionContext, diagnostics) => diagnostics.ForEach(productionContext.ReportDiagnostic));

        var (injectionDefinitionSuccessProvider, injectionDefinitionErrorProvider) = context.SyntaxProvider.SetupInjectionDefinitionStage().SegregateByResult();

        context.RegisterSourceOutput(injectionDefinitionErrorProvider, (productionContext, diagnostics) => diagnostics.ForEach(productionContext.ReportDiagnostic));

        var injectionTreeInputProvider = injectionDefinitionSuccessProvider
            .Combine(successCompilationDataProvider)
            .Combine(accessibleConstructorProvider.Collect())
            .Select((x, _) => (x.Left.Left, x.Left.Right, x.Right));

        var (factoryDefinitionSuccessProvider, factoryDefinitionErrorProvider) = injectionTreeInputProvider.SetupResolveFactoryGraphStage().SegregateByResult();

        var factoryDefinitionProvider = factoryDefinitionSuccessProvider.Combine(successCompilationDataProvider);

        context.RegisterSourceOutput(factoryDefinitionErrorProvider, (productionContext, error) => error.ForEach(productionContext.ReportDiagnostic));

        var (codeGeneratedFactorySuccessProvider, codeGeneratedFactoryErrorProvider) = factoryDefinitionProvider.SetupFactoryCodeGenerationStage().SegregateByResult();

        codeGeneratedFactorySuccessProvider.Select((x, _) => x.GeneratedFactoryOutputs).SetupOutputResultStage(context);
        context.RegisterSourceOutput(codeGeneratedFactoryErrorProvider, (productionContext, error) => error.ForEach(productionContext.ReportDiagnostic));

        var resolverCreationDefinitionsProvider = injectionDefinitionSuccessProvider.Select((x, _) => x.ResolverCreationDefinitions);
        var resolverCreationDefinitionAndFactoriesProvider = resolverCreationDefinitionsProvider
            .Combine(codeGeneratedFactorySuccessProvider.Select((x, _) => x.GeneratedTypeDeclaration).Collect()).Combine(successCompilationDataProvider)
            .Select((x, _) => (x.Left.Left, x.Left.Right, x.Right));

        var (codeGeneratedTypeResolversSuccessProvider, codeGeneratedTypeResolversErrorProvider) = resolverCreationDefinitionAndFactoriesProvider.SetupResolveTypeResolverStage().SegregateByResult();
        context.RegisterSourceOutput(codeGeneratedTypeResolversErrorProvider, (productionContext, error) => error.ForEach(productionContext.ReportDiagnostic));

        var codeGeneratedTypeResolverProvider = codeGeneratedTypeResolversSuccessProvider.Combine(successCompilationDataProvider).SetupTypeResolverCodeGeneration();

        codeGeneratedTypeResolverProvider.SetupOutputResultStage(context);
    }
}