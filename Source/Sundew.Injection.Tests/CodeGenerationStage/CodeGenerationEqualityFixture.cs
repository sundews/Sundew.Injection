﻿namespace Sundew.Injection.Tests.CodeGenerationStage;

extern alias sbt;
extern alias sig;
using FluentAssertions;
using FluentAssertions.Execution;
using sbt::Sundew.Base.Text;
using Sundew.Injection.Testing;
using CompilationDataProvider = sig::Sundew.Injection.Generator.Stages.CompilationDataStage.CompilationDataProvider;
using FactoryCodeGenerationProvider = sig::Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.FactoryCodeGenerationProvider;
using FactoryResolvedGraphProvider = sig::Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.FactoryResolvedGraphProvider;

[TestFixture]
public class CodeGenerationEqualityFixture
{
    [Test]
    public void Equals_Then_ResultShouldBeTrue()
    {
        var compilation = TestProjects.Success.FromCurrentDirectory.Value;
        var demoModuleDeclaration = compilation.GetTypeByMetadataName("OverallSuccess.InjectionDeclaration");
        if (demoModuleDeclaration == null)
        {
            Assert.Fail($"Could not find InjectionDeclaration. Compilation had: {compilation.GetDiagnostics().Length} diagnostics");
            throw new NotImplementedException("Assert.Fail is marked as throws.");
        }

        var injectionDefinitionSemanticModel = compilation.GetSemanticModel(demoModuleDeclaration.DeclaringSyntaxReferences.First().SyntaxTree, true);
        var injectionDefinition = sig::Sundew.Injection.Generator.Stages.InjectionDefinitionStage.InjectionDefinitionProvider.GetInjectionDefinition(injectionDefinitionSemanticModel, CancellationToken.None);
        if (!injectionDefinition.IsSuccess)
        {
            throw new AssertionFailedException($"InjectionDefinition should have been successful, but failed with errors: {injectionDefinition.Error!.JoinToString((builder, item) => builder.Append(item), ", ")}");
        }

        var compilationData = CompilationDataProvider.GetCompilationData(compilation, CancellationToken.None).Value!;

        var factoryDataArray = FactoryResolvedGraphProvider.GetResolvedFactoryGraph(injectionDefinition.Value!, compilationData, CancellationToken.None);

        var lhs = factoryDataArray.Select(x => FactoryCodeGenerationProvider.GetGeneratedOutput(x.Value!, compilationData, CancellationToken.None));
        var rhs = factoryDataArray.Select(x => FactoryCodeGenerationProvider.GetGeneratedOutput(x.Value!, compilationData, CancellationToken.None));

        lhs.Should().Equal(rhs);
    }
}