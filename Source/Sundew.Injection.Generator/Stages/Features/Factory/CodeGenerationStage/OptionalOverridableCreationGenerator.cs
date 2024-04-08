// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionalOverridableCreationGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage;

using Sundew.Base;
using Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;
using InvocationExpressionBase = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.InvocationExpressionBase;

internal sealed class OptionalOverridableCreationGenerator
{
    private readonly GeneratorFeatures generatorFeatures;
    private readonly GeneratorContext generatorContext;

    public OptionalOverridableCreationGenerator(GeneratorFeatures generatorFeatures, GeneratorContext generatorContext)
    {
        this.generatorFeatures = generatorFeatures;
        this.generatorContext = generatorContext;
    }

    public (FactoryNode FactoryNode, InvocationExpressionBase CreationExpression)
        Generate(IMayOverrideNewNode mayOverrideNew, in FactoryNode factoryNode)
    {
        return mayOverrideNew.OverridableNewParametersOption.GetValueOrDefault(
            factoryNode.FactoryImplementation.FactoryMethods,
            factoryNode,
            (creationParameters, factoryMethods, factoryNode) => this.generatorFeatures.OnCreateMethodGenerator.Generate(factoryMethods, mayOverrideNew.ReferencedType, creationParameters, mayOverrideNew.CreationSource, factoryNode),
            (factoryMethods, factoryNode) =>
            {
                var creationResult = this.generatorFeatures.CreationExpressionGenerator.Generate(factoryNode, mayOverrideNew.CreationSource, factoryNode.DependantArguments);
                return creationResult;
            });
    }
}