// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreationExpressionGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System.Collections.Generic;
using System.Linq;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.FactoryDataStage;

internal sealed class CreationExpressionGenerator
{
    private readonly GeneratorFeatures generatorFeatures;

    public CreationExpressionGenerator(GeneratorFeatures generatorFeatures, GeneratorContext generatorContext)
    {
        this.generatorFeatures = generatorFeatures;
    }

    public (FactoryNode FactoryNode, InvocationExpressionBase CreationExpression) Generate(
        in FactoryNode factoryNode,
        CreationSource creationSource,
        IReadOnlyList<Expression> arguments)
    {
        return creationSource switch
        {
            CreationSource.ArrayCreation arrayCreation => (factoryNode, new CreationExpression.Array(arrayCreation.ElementType, arguments)),
            CreationSource.ConstructorCall constructorCall => (factoryNode, new CreationExpression.ConstructorCall(constructorCall.Type, arguments)),
            CreationSource.DefaultValue defaultValue => (factoryNode, new CreationExpression.DefaultValue(defaultValue.DefiniteType)),
            CreationSource.InstanceMethodCall instanceMethodCall => this.GenerateInstanceMethodCallExpression(instanceMethodCall, arguments, in factoryNode),
            CreationSource.LiteralValue literalValue => (factoryNode, CreationExpression.Literal(literalValue.Literal)),
            CreationSource.StaticMethodCall staticMethodCall => (factoryNode, CreationExpression._StaticMethodCall(staticMethodCall.Type, staticMethodCall.Method.Name, staticMethodCall.Method.TypeArguments, arguments)),
        };
    }

    private (FactoryNode FactoryNode, InvocationExpressionBase CreationExpression) GenerateInstanceMethodCallExpression(
        CreationSource.InstanceMethodCall instanceMethodCall,
        IReadOnlyList<Expression> arguments,
        in FactoryNode factoryNode)
    {
        var factoryFactoryNode = this.generatorFeatures.InjectionNodeExpressionGenerator.Generate(instanceMethodCall.Instance, factoryNode.FactoryImplementation, factoryNode.CreateMethod);
        if (instanceMethodCall.IsProperty)
        {
            return (factoryFactoryNode, InvocationExpressionBase.MemberAccessExpression(factoryFactoryNode.DependeeArguments.Single(), instanceMethodCall.Method.Name));
        }

        return (factoryFactoryNode, CreationExpression._InstanceMethodCall(factoryFactoryNode.DependeeArguments.Single(), instanceMethodCall.Method.Name, instanceMethodCall.Method.TypeArguments, arguments));
    }
}