// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleInstancePerRequestGenerator.cs" company="Sundews">
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

internal class SingleInstancePerRequestGenerator
{
    private const string Owned = "owned";
    private readonly GeneratorFeatures generatorFeatures;
    private readonly GeneratorContext generatorContext;

    public SingleInstancePerRequestGenerator(
        GeneratorFeatures generatorFeatures,
        GeneratorContext generatorContext)
    {
        this.generatorFeatures = generatorFeatures;
        this.generatorContext = generatorContext;
    }

    public FactoryNode
        VisitSingleInstancePerRequest(
            SingleInstancePerRequestInjectionNode singleInstancePerRequestInjectionNode,
            in FactoryImplementation factoryImplementation,
            in MethodImplementation method)
    {
        var factoryNode = new FactoryNode(in factoryImplementation, in method, ImmutableList<Expression>.Empty);
        var targetType = singleInstancePerRequestInjectionNode.TargetType;
        var targetReferenceType = singleInstancePerRequestInjectionNode.TargetReferenceType;
        var variableType = singleInstancePerRequestInjectionNode.ParameterNodeOption.GetValueOrDefault(x => x.Type, targetReferenceType);
        (factoryNode, var wasAdded, var variableDeclaration) = factoryNode.GetOrAddVariable(
            NameHelper.GetIdentifierNameForType(variableType),
            variableType,
            (name) => new Declaration(variableType, name),
            (in FactoryNode factoryNode, bool willAdd, in Declaration declaration) =>
            {
                if (willAdd)
                {
                    return singleInstancePerRequestInjectionNode.Parameters.Aggregate(
                        factoryNode,
                        (factoryNode, nextCreationNode) =>
                        {
                            var factory = factoryNode.FactoryImplementation;
                            var factoryMethod = factoryNode.CreateMethod;
                            var result =
                                this.generatorFeatures.InjectionNodeExpressionGenerator.Generate(nextCreationNode, in factory, in factoryMethod);
                            return factoryNode with
                            {
                                FactoryImplementation = result.FactoryImplementation,
                                CreateMethod = result.CreateMethod,
                                DependeeArguments = factoryNode.DependeeArguments.AddRange(result.DependeeArguments),
                            };
                        });
                }

                return factoryNode;
            });

        var targetIdentifier = new Identifier(variableDeclaration.Name);
        if (wasAdded)
        {
            (factoryNode, var creationExpression) = this.generatorFeatures.OptionalOverridableCreationGenerator.Generate(singleInstancePerRequestInjectionNode, factoryNode);
            if (singleInstancePerRequestInjectionNode.ParameterNodeOption.TryGetValue(out var parameterNode))
            {
                (factoryNode, _, var parameter, var argument, _) = factoryNode.GetOrAddCreateMethodParameter(parameterNode, variableDeclaration.Name, this.generatorContext.CompilationData);
                var localDeclarationStatement = new LocalDeclarationStatement(Owned + targetType.Name, creationExpression);
                var localDeclarationIdentifier = new Identifier(localDeclarationStatement.Name);
                var localDeclarationAssignmentStatement = new ExpressionStatement(new AssignmentExpression(new Identifier(parameter.Name), localDeclarationIdentifier));
                var trueStatements = ImmutableList.Create<Statement>(localDeclarationStatement);
                if (singleInstancePerRequestInjectionNode.NeedsLifecycleHandling)
                {
                    var addInvocationStatement = new ExpressionStatement(new InvocationExpression(this.generatorContext.KnownSyntax.ChildLifecycleHandler.TryAddMethod, new Expression[] { localDeclarationIdentifier }));
                    trueStatements = trueStatements.Add(addInvocationStatement);
                }

                trueStatements = trueStatements.Add(localDeclarationAssignmentStatement);
                factoryNode = factoryNode.AddCreateMethodStatement(Statement.CreateOptionalParameterIfStatement(argument, trueStatements));
            }
            else
            {
                var assignmentStatement = new LocalDeclarationStatement(variableDeclaration.Name, creationExpression);
                factoryNode = factoryNode.AddCreateMethodStatement(assignmentStatement);
                if (singleInstancePerRequestInjectionNode.NeedsLifecycleHandling)
                {
                    factoryNode = factoryNode.AddCreateMethodStatement(new ExpressionStatement(new InvocationExpression(this.generatorContext.KnownSyntax.ChildLifecycleHandler.TryAddMethod, new Expression[] { targetIdentifier })));
                }
            }
        }

        return factoryNode with { DependeeArguments = ImmutableList.Create<Expression>(targetIdentifier) };
    }
}