// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnCreateMethodGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System.Collections.Immutable;
using System.Linq;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.FactoryDataStage;
using Sundew.Injection.Generator.TypeSystem;
using MethodImplementation = Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.MethodImplementation;

internal sealed class OnCreateMethodGenerator
{
    private const string OnCreate = "OnCreate";
    private readonly GeneratorFeatures generatorFeatures;
    private readonly GeneratorContext generatorContext;

    public OnCreateMethodGenerator(GeneratorFeatures generatorFeatures, GeneratorContext generatorContext)
    {
        this.generatorFeatures = generatorFeatures;
        this.generatorContext = generatorContext;
    }

    public (ImmutableList<DeclaredMethodImplementation> FactoryMethods, InvocationExpressionBase CreationExpression, FactoryNode FactoryNode)
        Generate(
            ImmutableList<DeclaredMethodImplementation> factoryMethods, DefiniteType targetType, ValueArray<DefiniteParameter> parameters, CreationSource creationSource, FactoryNode factoryNode)
    {
        var declaration = new MethodDeclaration(DeclaredAccessibility.Protected, true, OnCreate + NameHelper.GetFactoryMethodName(targetType.Name), parameters.Select(x => new ParameterDeclaration(x.Type, x.Name, null)).ToImmutableList(), targetType);
        var existingFactoryMethod = factoryMethods.FirstOrDefault(x => x.Declaration == declaration);
        var resultingFactoryNode = factoryNode;
        if (Equals(existingFactoryMethod.Declaration, default))
        {
            var creationExpressionPair = this.generatorFeatures.CreationExpressionGenerator.Generate(factoryNode, creationSource, declaration.Parameters.Select(x => new Identifier(x.Name)).ToImmutableArray());
            factoryMethods = factoryMethods.Add(new DeclaredMethodImplementation(declaration, new MethodImplementation(declaration.Parameters, ImmutableList<Declaration>.Empty, ImmutableList.Create<Statement>(new ReturnStatement(creationExpressionPair.CreationExpression)))));
            resultingFactoryNode = creationExpressionPair.FactoryNode;
        }

        return (factoryMethods, new InvocationExpression(new MemberAccessExpression(Identifier.This, declaration.Name), factoryNode.DependeeArguments), resultingFactoryNode);
    }
}