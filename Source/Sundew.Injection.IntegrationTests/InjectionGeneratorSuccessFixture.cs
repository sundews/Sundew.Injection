// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionGeneratorSuccessFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.IntegrationTests;

using System.Threading.Tasks;
using AllFeaturesSuccessDependency;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Sundew.Injection.Generator;
using Sundew.Injection.Testing;
using VerifyNUnit;

[TestFixture]
public class InjectionGeneratorSuccessFixture
{
    [Test]
    public Task VerifyGeneratedSources()
    {
        var compilation = TestProjects.AllFeatureSuccess.FromCurrentDirectory.Value;
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new InjectionGenerator());

        generatorDriver = generatorDriver.RunGenerators(compilation);
        new DependencyFactory().Create();
        return Verifier.Verify(generatorDriver);
    }
}