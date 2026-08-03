// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestProjects.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Testing;
public static class TestProjects
{
    public static TestProject Success = new(@"TestProjects/OverallSuccess", "OverallSuccessDependency.dll");
    public static TestProject DistinctSuccess = new(@"TestProjects/DistinctSuccess");
    public static TestProject DistinctErrors = new(@"TestProjects/DistinctErrors");
    public static TestProject TestPlayground = new(@"TestProjects/TestPlayground");
}