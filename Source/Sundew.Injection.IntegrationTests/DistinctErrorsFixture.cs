// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DistinctErrorsFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.IntegrationTests;

using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Sundew.Injection.Generator;
using Sundew.Injection.Testing;
using VerifyNUnit;

[TestFixture]
public class DistinctErrorsFixture
{
    [Test]
    public Task VerifyReportedDiagnostics()
    {
        var compilation = TestProjects.DistinctErrors.FromCurrentDirectory.Value;
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new InjectionGenerator());

        generatorDriver = generatorDriver.RunGenerators(compilation);
        var runResult = generatorDriver.GetRunResult();
        var diagnostics = runResult.Diagnostics.OrderBy(x => x.Id).ToImmutableArray();
        runResult.GetType().GetField("_lazyDiagnostics", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(runResult, diagnostics);

        return Verifier.Verify(runResult).ScrubLinesWithReplace(x => Path.IsPathRooted(x) ? x.Replace('\\', '/') : x);
    }
}