// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewInstanceGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System.Collections.Immutable;
using System.Linq;
using Sundew.Base.Primitives.Computation;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;
using MethodImplementation = Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.MethodImplementation;

internal sealed class NewInstanceGenerator
{
    private readonly GeneratorContext generatorContext;
    private readonly GeneratorFeatures generatorFeatures;

    public NewInstanceGenerator(
        GeneratorFeatures generatorFeatures,
        GeneratorContext generatorContext)
    {
        this.generatorContext = generatorContext;
        this.generatorFeatures = generatorFeatures;
    }

    public FactoryNode
        VisitNewInstance(
            NewInstanceInjectionNode newInstanceInjectionNode,
            in FactoryImplementation factoryImplementation,
            in MethodImplementation createMethod)
    {
        var factoryNode = newInstanceInjectionNode.Parameters.Aggregate(
            new FactoryNode(in factoryImplementation, in createMethod, ImmutableList<Expression>.Empty),
            (factoryNode, nextCreationNode) =>
            {
                var factory = factoryNode.FactoryImplementation;
                var factoryMethod = factoryNode.CreateMethod;
                var result = this.generatorFeatures.InjectionNodeExpressionGenerator.Generate(nextCreationNode, in factory, in factoryMethod);
                return factoryNode with
                {
                    FactoryImplementation = result.FactoryImplementation,
                    CreateMethod = result.CreateMethod,
                    DependeeArguments = factoryNode.DependeeArguments.AddRange(result.DependeeArguments),
                };
            });

        var dependeeArguments = ImmutableList.Create<Expression>();
        var targetReferenceType = newInstanceInjectionNode.TargetReferenceType;

        (var factoryMethods, var creationExpression, factoryNode) = newInstanceInjectionNode.OverridableNewParametersOption.Evaluate(
            factoryNode.FactoryImplementation.FactoryMethods,
            (creationParameters, factoryMethods) => this.generatorFeatures.OnCreateMethodGenerator.Generate(factoryMethods, targetReferenceType, creationParameters, newInstanceInjectionNode.CreationSource, factoryNode),
            factoryMethods =>
            {
                var creationResult = this.generatorFeatures.CreationExpressionGenerator.Generate(in factoryNode, newInstanceInjectionNode.CreationSource, factoryNode.DependeeArguments);
                return (factoryMethods, creationResult.CreationExpression, creationResult.FactoryNode);
            });

        var variables = factoryNode.CreateMethod.Variables;
        var statements = factoryNode.CreateMethod.Statements;
        var variableDeclarationOption = O.From(
            newInstanceInjectionNode.NeedsLifecycleHandling || newInstanceInjectionNode.ParameterNodeOption.HasValue,
            () =>
            {
                var variableName = NameHelper.GetDependeeScopedName(newInstanceInjectionNode);
                return variables.GetOrAdd(
                    variableName,
                    targetReferenceType,
                    name => new Declaration(targetReferenceType, name));
            });

        var factoryMethodParameters = factoryNode.CreateMethod.Parameters;
        if (newInstanceInjectionNode.ParameterNodeOption.TryGetValue(out var parameterNode))
        {
            var (parameterDeclarations, _, parameter, argument, _) = ParameterHelper.VisitParameter(
                parameterNode,
                null,
                factoryMethodParameters,
                factoryImplementation.Constructor.Parameters,
                this.generatorContext.CompilationData);
            var (newVariables, _, declaration) = variableDeclarationOption.Value;
            variables = newVariables;
            var localDeclarationStatement = new LocalDeclarationStatement(declaration.Name, new NullCoalescingOperatorExpression(argument, creationExpression));
            statements = statements.Add(localDeclarationStatement);
            var localDeclarationIdentifier = new Identifier(localDeclarationStatement.Name);
            if (newInstanceInjectionNode.NeedsLifecycleHandling)
            {
                var addInvocationStatement = new ExpressionStatement(new InvocationExpression(this.generatorContext.KnownSyntax.ChildLifecycleHandler.TryAddMethod, new Expression[] { localDeclarationIdentifier }));
                statements = statements.Add(addInvocationStatement);
            }

            dependeeArguments = dependeeArguments.Add(localDeclarationIdentifier);
            factoryMethodParameters = parameterDeclarations;
        }
        else
        {
            if (variableDeclarationOption.HasValue)
            {
                var (newVariables, wasAdded, declaration) = variableDeclarationOption.Value;
                var variableIdentifier = new Identifier(declaration.Name);
                variables = newVariables;
                dependeeArguments = dependeeArguments.Add(variableIdentifier);
                if (wasAdded)
                {
                    statements = statements.Add(new LocalDeclarationStatement(variableIdentifier.Name, creationExpression))
                        .Add(new ExpressionStatement(new InvocationExpression(this.generatorContext.KnownSyntax.ChildLifecycleHandler.TryAddMethod, new Expression[] { variableIdentifier })));
                }
            }
            else
            {
                dependeeArguments = dependeeArguments.Add(creationExpression);
            }
        }

        return factoryNode with
        {
            CreateMethod = factoryNode.CreateMethod with
            {
                Variables = variables,
                Statements = statements,
                Parameters = factoryMethodParameters,
            },
            DependeeArguments = dependeeArguments,
            FactoryImplementation = factoryNode.FactoryImplementation with { FactoryMethods = factoryMethods },
        };
    }
}