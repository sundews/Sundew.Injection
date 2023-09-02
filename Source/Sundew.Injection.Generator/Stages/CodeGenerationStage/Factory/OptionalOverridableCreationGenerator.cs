// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionalOverridableCreationGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.FactoryDataStage;

internal sealed class OptionalOverridableCreationGenerator
{
    private readonly GeneratorFeatures generatorFeatures;
    private readonly GeneratorContext generatorContext;

    public OptionalOverridableCreationGenerator(GeneratorFeatures generatorFeatures, GeneratorContext generatorContext)
    {
        this.generatorFeatures = generatorFeatures;
        this.generatorContext = generatorContext;
    }

    public (ImmutableList<DeclaredMethodImplementation> FactoryMethods, InvocationExpressionBase CreationExpression, FactoryNode FactoryNode)
        Generate(IMayOverrideNewNode mayOverrideNew, in FactoryNode factoryNode)
    {
        return mayOverrideNew.OverridableNewParametersOption.Evaluate(
            factoryNode.FactoryImplementation.FactoryMethods,
            factoryNode,
            (creationParameters, factoryMethods, factoryNode) => this.generatorFeatures.OnCreateMethodGenerator.Generate(factoryMethods, mayOverrideNew.TargetReferenceType, creationParameters, mayOverrideNew.CreationSource, factoryNode),
            (factoryMethods, factoryNode) =>
            {
                var creationResult = this.generatorFeatures.CreationExpressionGenerator.Generate(factoryNode, mayOverrideNew.CreationSource, factoryNode.DependeeArguments);
                return (factoryMethods, creationResult.CreationExpression, creationResult.FactoryNode);
            });
    }
}