using FluentAssertions;

namespace Sundew.Injection.Tests.InjectionDefinitionStage;

using Generator.Stages.InjectionDefinitionStage;
using Testing;

[TestFixture]
public class InjectionDefinitionFixture
{
    [Test]
    public void Equals_Then_ResultShouldBeTrue()
    {
        var project = new Project(DemoProjectInfo.GetPath("AllFeaturesSuccess"), new Paths(DemoProjectInfo.GetPath("Sundew.Injection")), "bin", "obj");
        var compilation = project.Compile();
        var demoModuleDeclaration = compilation.GetTypeByMetadataName("AllFeaturesSuccess.DemoModuleDeclaration");
        if (demoModuleDeclaration == null)
        {
            Assert.Fail("Could not find DemoModuleDeclaration");
            throw new NotImplementedException("Assert doesn't mark Fail as throws.");
        }

        var injectionDefinitionSemanticModel = compilation.GetSemanticModel(demoModuleDeclaration.DeclaringSyntaxReferences.First().SyntaxTree, true);
        var lhs = InjectionDefinitionProvider.GetInjectionDefinition(injectionDefinitionSemanticModel, CancellationToken.None);
        var rhs = InjectionDefinitionProvider.GetInjectionDefinition(injectionDefinitionSemanticModel, CancellationToken.None);

        lhs.Should().Be(rhs);
    }
}