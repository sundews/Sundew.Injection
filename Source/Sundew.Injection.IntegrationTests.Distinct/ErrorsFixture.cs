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
using Sundew.Injection.Generator;
using Sundew.Injection.Testing;
using VerifyTUnit;

public class ErrorsFixture
{
    [Test]
    [Arguments(@"ImplementFactoryForNonNamedType")]
    [Arguments(@"NoBindingForNonInstantiableType")]
    [Arguments(@"NoExactParameterMatch")]
    [Arguments(@"NoFactoryMethodForBind")]
    [Arguments(@"NonGenericBind")]
    [Arguments(@"NonInstantiableType")]
    [Arguments(@"NoViableConstructorFound")]
    [Arguments(@"Recursive")]
    [Arguments(@"RequestedTypeVersusReferencedTypeMismatch")]
    [Arguments(@"ScopeError")]
    public Task VerifyGeneratedSources(string project)
    {
        var compilation = new TestProject($@"TestProjects\DistinctErrors\{project}").FromCurrentDirectory.Value;
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new InjectionGenerator());

        generatorDriver = generatorDriver.RunGenerators(compilation);
        return Verifier.Verify(generatorDriver);
    }
}