namespace Sundew.Injection.Tests.InjectionDefinitionStage;

using FluentAssertions;
using Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Testing;

[TestFixture]
public class InjectionDefinitionEqualityFixture
{
    [Test]
    public void Equals_Then_ResultShouldBeTrue()
    {
        var compilation = TestProjects.Success.FromCurrentDirectory.Value;
        var demoModuleDeclaration = compilation.GetTypeByMetadataName("Success.InjectionDeclaration");
        if (demoModuleDeclaration == null)
        {
            Assert.Fail($"Could not find InjectionDeclaration. Compilation had: {compilation.GetDiagnostics().Length} diagnostics");
            throw new NotImplementedException("Assert.Fail is marked as throws.");
        }

        var injectionDefinitionSemanticModel = compilation.GetSemanticModel(demoModuleDeclaration.DeclaringSyntaxReferences.First().SyntaxTree, true);
        var lhs = InjectionDefinitionProvider.GetInjectionDefinition(injectionDefinitionSemanticModel, CancellationToken.None);
        var rhs = InjectionDefinitionProvider.GetInjectionDefinition(injectionDefinitionSemanticModel, CancellationToken.None);

        lhs.Should().Be(rhs);
    }
}