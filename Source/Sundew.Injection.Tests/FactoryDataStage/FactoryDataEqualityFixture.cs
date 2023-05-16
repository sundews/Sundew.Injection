namespace Sundew.Injection.Tests.FactoryDataStage;

extern alias sbt;

using FluentAssertions;
using FluentAssertions.Execution;
using sbt::Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Testing;

[TestFixture]
public class FactoryDataEqualityFixture
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
        var injectionDefinition= InjectionDefinitionProvider.GetInjectionDefinition(injectionDefinitionSemanticModel, CancellationToken.None);
        if (!injectionDefinition.IsSuccess)
        {
            throw new AssertionFailedException($"InjectionDefinition should have been successful, but failed with errors: {injectionDefinition.Error.JoinToString((builder, item) => builder.Append(item), ", ")}");
        }

        var compilationData = CompilationDataProvider.GetCompilationData(compilation);

        var lhs= FactoryDataProvider.GetFactoryData(injectionDefinition.Value, compilationData, CancellationToken.None);
        var rhs = FactoryDataProvider.GetFactoryData(injectionDefinition.Value, compilationData, CancellationToken.None);

        lhs.Should().Equal(rhs);
    }
}