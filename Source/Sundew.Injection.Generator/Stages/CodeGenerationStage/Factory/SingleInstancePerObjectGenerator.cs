// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleInstancePerObjectGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

internal class SingleInstancePerObjectGenerator
{
    private readonly GeneratorFeatures generatorFeatures;
    private readonly GeneratorContext generatorContext;

    public SingleInstancePerObjectGenerator(
        GeneratorFeatures generatorFeatures,
        GeneratorContext generatorContext)
    {
        this.generatorFeatures = generatorFeatures;
        this.generatorContext = generatorContext;
    }

    public FactoryNode VisitSingleInstancePerObject(
        SingleInstancePerObjectInjectionNode singleInstancePerObjectInjectionNode,
        in FactoryImplementation factoryImplementation,
        in MethodImplementation method)
    {
        throw new NotImplementedException();
    }
}