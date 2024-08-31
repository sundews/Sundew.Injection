namespace Sundew.Injection.Tests.Playground;

extern alias sbt;
extern alias sig;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using sbt::Sundew.Base.Text;
using Sundew.Injection.Testing;
using InjectionGenerator = sig::Sundew.Injection.Generator.InjectionGenerator;

[TestFixture]
public class Playground
{
    [Test, Ignore("Only when using playground")]
    public void Test()
    {
        var compilation = TestProjects.TestPlayground.FromCurrentDirectory.Value;
        var demoModuleDeclaration = compilation.GetTypeByMetadataName("TestPlayground.InjectionDeclaration");
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
    }

    [Test]
    public Task InjectionGeneratorTest()
    {
        var compilation = TestProjects.TestPlayground.FromCurrentDirectory.Value;
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new InjectionGenerator());

        generatorDriver = generatorDriver.RunGenerators(compilation);
        return Verifier.Verify(generatorDriver);
    }
}