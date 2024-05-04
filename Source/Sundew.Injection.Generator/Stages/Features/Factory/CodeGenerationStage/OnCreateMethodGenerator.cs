// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnCreateMethodGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage;

using System.Collections.Immutable;
using System.Linq;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;
using Sundew.Injection.Generator.TypeSystem;
using CreationSource = Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.CreationSource;
using InvocationExpressionBase = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.InvocationExpressionBase;
using MethodImplementation = Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model.MethodImplementation;
using Statement = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Statement;

internal sealed class OnCreateMethodGenerator
{
    private const string OnCreate = "OnCreate";
    private readonly GeneratorContext generatorContext;
    private readonly GeneratorFeatures generatorFeatures;

    public OnCreateMethodGenerator(GeneratorFeatures generatorFeatures, GeneratorContext generatorContext)
    {
        this.generatorContext = generatorContext;
        this.generatorFeatures = generatorFeatures;
    }

    public (FactoryNode FactoryNode, InvocationExpressionBase CreationExpression)
        Generate(
            ImmutableList<DeclaredMethodImplementation> factoryMethods,
            Type targetType,
            ValueArray<FullParameter> parameters,
            CreationSource creationSource,
            KnownSyntax.LifecycleHandlerSyntax lifecycleHandlerSyntax,
            FactoryNode factoryNode)
    {
        var parameterDeclarations = parameters.Select(x => new ParameterDeclaration(x.Type, x.Name)).ToImmutableList();
        parameterDeclarations = parameterDeclarations.Add(lifecycleHandlerSyntax.OnCreateMethodParameterDeclaration);

        var declaration = new MethodDeclaration(DeclaredAccessibility.Protected, true, OnCreate + NameHelper.GetFactoryMethodName(targetType.Name), parameterDeclarations, new UsedType(targetType));
        var existingFactoryMethod = factoryMethods.FirstOrDefault(x => x.Declaration == declaration);
        var resultingFactoryNode = factoryNode;
        if (Equals(existingFactoryMethod.Declaration, default))
        {
            var creationExpressionPair = this.generatorFeatures.CreationExpressionGenerator.Generate(factoryNode, creationSource, parameters.Select(x => new Identifier(x.Name)).ToImmutableArray());
            factoryMethods = factoryMethods.Add(new DeclaredMethodImplementation(declaration, new MethodImplementation(declaration.Parameters, ImmutableList<Declaration>.Empty, ImmutableList.Create<Statement>(new ReturnStatement(creationExpressionPair.CreationExpression)))));
            resultingFactoryNode = creationExpressionPair.FactoryNode;
        }

        return (resultingFactoryNode with { FactoryImplementation = resultingFactoryNode.FactoryImplementation with { FactoryMethods = factoryMethods } }, new InvocationExpression(new MemberAccessExpression(Identifier.This, declaration.Name), factoryNode.DependantArguments.Add(lifecycleHandlerSyntax.Access)));
    }
}