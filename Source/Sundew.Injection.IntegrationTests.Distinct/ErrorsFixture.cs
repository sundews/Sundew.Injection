// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorsFixture.cs" company="Sundews">
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
public class ErrorsFixture
{
    [TestCase(@"ImplementFactoryForNonNamedType")]
    [TestCase(@"NoBindingForNonInstantiableType")]
    [TestCase(@"NoExactParameterMatch")]
    [TestCase(@"NoFactoryMethodForBind")]
    [TestCase(@"NonGenericBind")]
    [TestCase(@"NonInstantiableType")]
    [TestCase(@"NoViableConstructorFound")]
    [TestCase(@"Recursive")]
    [TestCase(@"RequestedTypeVersusReferencedTypeMismatch")]
    [TestCase(@"ScopeError")]
    public Task VerifyGeneratedSources(string project)
    {
        var compilation = new TestProject($@"TestProjects\DistinctErrors\{project}").FromCurrentDirectory.Value;
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new InjectionGenerator());

        generatorDriver = generatorDriver.RunGenerators(compilation);
        return Verifier.Verify(generatorDriver);
    }
}