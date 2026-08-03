// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuccessFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.IntegrationTests.Distinct;

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sundew.Injection.Generator;
using Sundew.Injection.Testing;
using VerifyTUnit;

public class SuccessFixture
{
    [Test]
    [Arguments(@"AllowingOverrideNew")]
    [Arguments(@"DependencyFromBoundFactoryWithDisposable")]
    [Arguments(@"DependencyFromBoundInterfaceFactory")]
    [Arguments(@"DisposableDependency")]
    [Arguments(@"DisposableFactoriesAllowingOverrideNew")]
    [Arguments(@"InitializableDependency")]
    [Arguments(@"IntermediateDependencyFromBoundFactoryWithInitializable")]
    [Arguments(@"MultipleFactoryMethods")]
    [Arguments(@"MultipleParameters")]
    [Arguments(@"Parameters\ConstructorOptionalReferenceType")]
    [Arguments(@"Parameters\OptionalInt")]
    [Arguments(@"Parameters\OptionalIntToRequired")]
    [Arguments(@"Parameters\OptionalLifecycle")]
    [Arguments(@"Parameters\OptionalLifecycleWithDefaultValue")]
    [Arguments(@"Parameters\OptionalLifecycleWithoutRegistration")]
    [Arguments(@"Parameters\OptionalReferenceType")]
    [Arguments(@"Parameters\OptionalString")]
    [Arguments(@"PartialConstructor")]
    [Arguments(@"PartialProperty")]
    [Arguments(@"SelectedConstructor")]
    [Arguments(@"SingletonFactory")]
    public Task VerifyGeneratedSources(string project)
    {
        var compilation = new TestProject($@"TestProjects\DistinctSuccess\{project}").FromCurrentDirectory.Value;
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new InjectionGenerator());

        generatorDriver = generatorDriver.RunGenerators(compilation);

        var verifySettings = new VerifyTests.VerifySettings();
        verifySettings.UseFileName($"SuccessFixture.Verify.{project.Replace('\\', '-')}");
        return Verifier.Verify(generatorDriver, verifySettings);
    }
}