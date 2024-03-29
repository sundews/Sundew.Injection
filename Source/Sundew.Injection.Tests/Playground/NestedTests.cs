﻿namespace Sundew.Injection.Tests.Playground;

extern alias sbt;
using FluentAssertions.Execution;
using sbt::Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Testing;

[TestFixture]
public class NestedTests
{
    [Test]
    public void T()
    {
        var compilation = TestProjects.TestPlayground.FromCurrentDirectory.Value;
        var demoModuleDeclaration = compilation.GetTypeByMetadataName("TestPlayground.FactoryDeclaration");
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
    }
}