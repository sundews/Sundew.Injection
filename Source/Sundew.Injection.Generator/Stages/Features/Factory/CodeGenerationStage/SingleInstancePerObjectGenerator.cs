// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleInstancePerObjectGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage;

using System;
using Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;

internal class SingleInstancePerObjectGenerator(
    GeneratorFeatures generatorFeatures,
    GeneratorContext generatorContext)
{
    private readonly GeneratorFeatures generatorFeatures = generatorFeatures;
    private readonly GeneratorContext generatorContext = generatorContext;

    public FactoryNode VisitSingleInstancePerObject(
        SingleInstancePerObjectInjectionNode singleInstancePerObjectInjectionNode,
        in FactoryImplementation factoryImplementation,
        in MethodImplementation method)
    {
        throw new NotImplementedException();
    }
}