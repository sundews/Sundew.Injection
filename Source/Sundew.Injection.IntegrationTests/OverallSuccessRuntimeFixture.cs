// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OverallSuccessRuntimeFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.IntegrationTests;

using System.Threading.Tasks;
using NUnit.Framework;
using OverallSuccess;
using OverallSuccess.OptionalInterface;
using OverallSuccess.RequiredInterface;
using OverallSuccessDependency;
using VerifyNUnit;

[TestFixture]
public class OverallSuccessRuntimeFixture
{
    [Test]
    public Task FactoryLifetimeTest()
    {
        var injectedSeparately1 = new InjectedSeparately();
        var injectedSeparately2 = new InjectedSeparately();
        using (var resolveRootFactory = new ResolveRootFactory(
                   new RequiredParameters(
                       new MultipleModuleRequiredParameter1(),
                       new MultipleModuleRequiredParameter2(),
                       new SingleModuleRequiredParameterConstructorMethod(),
                       new SingleModuleRequiredParameterCreateMethod()),
                   injectedSeparately1,
                   new InjectedByType(),
                   injectedSeparately2,
                   new OptionalParameters(),
                   "Name"))
        {
            var resolveRoot = resolveRootFactory.CreateResolveRoot(new[] { 1, 2, 3, 4 }, 5, () => new RequiredService(), new RequiredParameter("Test"));
        }

        return Verifier.Verify(FactoryLifetime.Log.ToString());
    }
}