// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryConstructorParameterGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage;

using System.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;
using MethodImplementation = Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model.MethodImplementation;

internal sealed class FactoryConstructorParameterGenerator
{
    private readonly GeneratorFeatures generatorFeatures;
    private readonly GeneratorContext generatorContext;

    public FactoryConstructorParameterGenerator(
        GeneratorFeatures generatorFeatures,
        GeneratorContext generatorContext)
    {
        this.generatorFeatures = generatorFeatures;
        this.generatorContext = generatorContext;
    }

    public FactoryNode
        VisitFactoryConstructorParameter(
            FactoryConstructorParameterInjectionNode factoryConstructorParameterInjectionNode,
            in FactoryImplementation factoryImplementation,
            in MethodImplementation method)
    {
        var factoryNode = new FactoryNode(in factoryImplementation, in method, ImmutableList<Expression>.Empty);

        var (argument, _) = ParameterHelper.VisitParameter(
            factoryConstructorParameterInjectionNode,
            this.generatorContext.CompilationData);

        var identifier = new Identifier(factoryConstructorParameterInjectionNode.ParameterSource.Name);

        var statements = factoryImplementation.Constructor.Statements;
        var isOptional = this.generatorContext.FactoryResolvedGraph.ReferencedTypesOptionality.TryGetValue(
            new RequestedParameter(factoryConstructorParameterInjectionNode.ParameterSource.Type.Id, factoryConstructorParameterInjectionNode.ParameterSource.Name), out var value) && value;
        (factoryNode, var wasAdded, var parameterField) = factoryNode.GetOrAddField(
            factoryConstructorParameterInjectionNode.ParameterSource.Name,
            factoryConstructorParameterInjectionNode.ParameterSource.Type,
            (fieldName) => new FieldDeclaration(
                factoryConstructorParameterInjectionNode.ParameterSource.Type,
                fieldName,
                isOptional,
                FieldModifier.Instance));

        if (wasAdded)
        {
            var assignmentStatement =
                new ExpressionStatement(new AssignmentExpression(
                    new MemberAccessExpression(Identifier.This, parameterField.Name), identifier));
            statements = statements.Add(assignmentStatement);
        }

        return factoryNode with
        {
            FactoryImplementation = factoryNode.FactoryImplementation with
            {
                Constructor = factoryNode.FactoryImplementation.Constructor with
                {
                    Statements = statements,
                },
            },
            DependantArguments = ImmutableList.Create(argument),
        };
    }
}