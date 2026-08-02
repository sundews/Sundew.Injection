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
using NUnit.Framework;
using Sundew.Injection.Generator;
using Sundew.Injection.Testing;
using VerifyNUnit;

[TestFixture]
public class SuccessFixture
{
    [TestCase(@"AllowingOverrideNew")]
    [TestCase(@"DependencyFromBoundFactoryWithDisposable")]
    [TestCase(@"DependencyFromBoundInterfaceFactory")]
    [TestCase(@"DisposableDependency")]
    [TestCase(@"DisposableFactoriesAllowingOverrideNew")]
    [TestCase(@"InitializableDependency")]
    [TestCase(@"IntermediateDependencyFromBoundFactoryWithInitializable")]
    [TestCase(@"MultipleFactoryMethods")]
    [TestCase(@"MultipleParameters")]
    [TestCase(@"Parameters\ConstructorOptionalReferenceType")]
    [TestCase(@"Parameters\OptionalInt")]
    [TestCase(@"Parameters\OptionalIntToRequired")]
    [TestCase(@"Parameters\OptionalLifecycle")]
    [TestCase(@"Parameters\OptionalLifecycleWithDefaultValue")]
    [TestCase(@"Parameters\OptionalLifecycleWithoutRegistration")]
    [TestCase(@"Parameters\OptionalReferenceType")]
    [TestCase(@"Parameters\OptionalString")]
    [TestCase(@"PartialConstructor")]
    [TestCase(@"PartialProperty")]
    [TestCase(@"SelectedConstructor")]
    [TestCase(@"SingletonFactory")]
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