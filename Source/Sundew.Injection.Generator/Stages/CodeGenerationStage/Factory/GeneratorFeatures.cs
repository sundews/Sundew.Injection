// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratorFeatures.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

internal sealed class GeneratorFeatures
{
    public GeneratorFeatures(GeneratorContext generatorContext)
    {
        this.InjectionNodeExpressionGenerator = new InjectionNodeEvaluator(this, generatorContext);
        this.OnCreateMethodGenerator = new OnCreateMethodGenerator(this, generatorContext);
        this.CreationExpressionGenerator = new CreationExpressionGenerator(this, generatorContext);
        this.OptionalOverridableCreationGenerator = new OptionalOverridableCreationGenerator(this, generatorContext);
    }

    public InjectionNodeEvaluator InjectionNodeExpressionGenerator { get; }

    public OptionalOverridableCreationGenerator OptionalOverridableCreationGenerator { get; }

    public OnCreateMethodGenerator OnCreateMethodGenerator { get; }

    public CreationExpressionGenerator CreationExpressionGenerator { get; }
}