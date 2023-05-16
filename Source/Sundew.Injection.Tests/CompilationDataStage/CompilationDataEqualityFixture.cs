namespace Sundew.Injection.Tests.CompilationDataStage;

extern alias sbt;

using FluentAssertions;
using FluentAssertions.Execution;
using sbt::Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Testing;

public class CompilationDataEqualityFixture
{
    [Test]
    public void Equals_Then_ResultShouldBeTrue()
    {
        var compilation = TestProjects.AllFeatureSuccess.FromCurrentDirectory.Value;
        var demoModuleDeclaration = compilation.GetTypeByMetadataName("AllFeaturesSuccess.FactoryDeclaration");
        if (demoModuleDeclaration == null)
        {
            Assert.Fail($"Could not find FactoryDeclaration. Compilation had: {compilation.GetDiagnostics().Length} diagnostics");
            throw new NotImplementedException("Assert.Fail is marked as throws.");
        }

        var injectionDefinitionSemanticModel = compilation.GetSemanticModel(demoModuleDeclaration.DeclaringSyntaxReferences.First().SyntaxTree, true);
        var injectionDefinition = InjectionDefinitionProvider.GetInjectionDefinition(injectionDefinitionSemanticModel, CancellationToken.None);
        if (!injectionDefinition.IsSuccess)
        {
            throw new AssertionFailedException($"InjectionDefinition should have been successful, but failed with errors: {injectionDefinition.Error.JoinToString((builder, item) => builder.Append(item), ", ")}");
        }

        var lhs = CompilationDataProvider.GetCompilationData(compilation);
        var rhs = CompilationDataProvider.GetCompilationData(compilation);

        lhs.Should().Be(rhs);
    }
}