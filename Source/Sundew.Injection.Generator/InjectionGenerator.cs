// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator;

using Microsoft.CodeAnalysis;
using Sundew.Base.Collections;
using Sundew.Injection.Generator.Stages.CodeGenerationStage;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.Stages.OutputStage;
using Sundew.Injection.Generator.Stages.SemanticModelStage;

[Generator]
public class InjectionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var accessibleConstructorProvider = context.SyntaxProvider.SetupAccessibleConstructorStage();

        var (successCompilationInfoProvider, errorCompilationInfoProvider) = context.CompilationProvider.SetupCompilationDataStage().SegregateByResult();

        context.RegisterSourceOutput(errorCompilationInfoProvider, (productionContext, diagnostics) => diagnostics.ForEach(productionContext.ReportDiagnostic));

        var (injectionDefinitionSuccessProvider, injectionDefinitionErrorProvider) = context.SyntaxProvider.SetupInjectionDefinitionStage().SegregateByResult();

        context.RegisterSourceOutput(injectionDefinitionErrorProvider, (productionContext, diagnostics) => diagnostics.ForEach(productionContext.ReportDiagnostic));

        var injectionTreeInputProvider = injectionDefinitionSuccessProvider
            .Combine(successCompilationInfoProvider)
            .Combine(accessibleConstructorProvider.Collect())
            .Select((x, _) => (x.Left.Left, x.Left.Right, x.Right));

        var (factoryDefinitionSuccessProvider, factoryDefinitionErrorProvider) = injectionTreeInputProvider.SetupFactoryDataStage().SegregateByResult();

        var factoryDefinitionProvider = factoryDefinitionSuccessProvider.Combine(successCompilationInfoProvider);

        context.RegisterSourceOutput(factoryDefinitionErrorProvider, (productionContext, error) => error.ForEach(productionContext.ReportDiagnostic));

        var (codeGenerationSuccessProvider, codeGenerationErrorProvider) = GeneratedCodeProvider.SetupCodeGenerationStage(factoryDefinitionProvider).SegregateByResult();

        codeGenerationSuccessProvider.SetupOutputResultStage(context);

        context.RegisterSourceOutput(codeGenerationErrorProvider, (productionContext, error) => error.ForEach(productionContext.ReportDiagnostic));
    }
}