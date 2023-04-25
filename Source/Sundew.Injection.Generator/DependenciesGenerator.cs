// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependenciesGenerator.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator;

using Microsoft.CodeAnalysis;
using Sundew.Injection.Generator.Stages.ProvideDependenciesStage;

[Generator]
public class DependenciesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.ProvideDependencies();
    }
}