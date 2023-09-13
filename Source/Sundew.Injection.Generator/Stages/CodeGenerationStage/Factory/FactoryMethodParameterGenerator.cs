// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethodParameterGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

internal class FactoryMethodParameterGenerator
{
    private readonly GeneratorFeatures generatorFeatures;
    private readonly GeneratorContext generatorContext;

    public FactoryMethodParameterGenerator(
        GeneratorFeatures generatorFeatures,
        GeneratorContext generatorContext)
    {
        this.generatorFeatures = generatorFeatures;
        this.generatorContext = generatorContext;
    }

    public FactoryNode VisitFactoryMethodParameter(
        FactoryMethodParameterInjectionNode factoryMethodParameterInjectionNode,
        in FactoryImplementation factoryImplementation,
        in MethodImplementation method)
    {
        var (parameters, _, _, argument, _) = ParameterHelper.VisitParameter(
            factoryMethodParameterInjectionNode,
            null,
            method.Parameters,
            factoryImplementation.Constructor.Parameters,
            this.generatorContext.CompilationData);
        return new FactoryNode(
            factoryImplementation,
            method with
            {
                Parameters = parameters,
            },
            ImmutableList.Create(argument));
    }
}