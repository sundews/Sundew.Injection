// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionGeneratorErrorFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
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
public class InjectionGeneratorErrorFixture
{
    [Test]
    public Task VerifyReportedDiagnostics()
    {
        var compilation = TestProjects.NetStandardLibraryErrors.FromCurrentDirectory.Value;
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new InjectionGenerator());

        generatorDriver = generatorDriver.RunGenerators(compilation);

        return Verifier.Verify(generatorDriver);
    }
}