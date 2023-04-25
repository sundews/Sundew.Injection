// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionGeneratorFixture.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.IntegrationTests;

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Sundew.Injection.Generator;
using Sundew.Injection.Testing;
using VerifyNUnit;

[TestFixture]
public class InjectionGeneratorFixture
{
    [Test]
    public Task VerifyGeneratedSources()
    {
        var project = new Testing.Project(DemoProjectInfo.GetPath("AllFeaturesSuccess"), new Paths(DemoProjectInfo.GetPath("Sundew.Injection")), "bin", "obj");
        var compilation = project.Compile();
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new InjectionGenerator());

        generatorDriver = generatorDriver.RunGenerators(compilation);

        return Verifier.Verify(generatorDriver);
    }
}