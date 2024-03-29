// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleInstancePerRequestGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage;

using System.Collections.Immutable;
using System.Linq;
using Sundew.Base;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;
using Expression = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Expression;
using MethodImplementation = Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model.MethodImplementation;

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
        var targetReferenceType = singleInstancePerRequestInjectionNode.ReferencedType;
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
                (factoryNode, _, var parameter, var parameterArgument, _) = factoryNode.GetOrAddCreateMethodParameter(parameterNode, variableDeclaration.Name, this.generatorContext.CompilationData);
                if (singleInstancePerRequestInjectionNode.NeedsLifecycleHandling)
                {
                    creationExpression = new InvocationExpression(this.generatorContext.KnownSyntax.ChildLifecycleHandler.TryAddMethod, new Expression[] { creationExpression });
                }

                factoryNode = factoryNode.AddCreateMethodStatement(Statement.ExpressionStatement(Expression.NullCoalescingOperatorExpression(parameterArgument, creationExpression, true)));
            }
            else
            {
                var assignmentStatement = new LocalDeclarationStatement(variableDeclaration.Name, creationExpression);
                factoryNode = factoryNode.AddCreateMethodStatement(assignmentStatement);
                if (singleInstancePerRequestInjectionNode.NeedsLifecycleHandling)
                {
                    factoryNode = factoryNode.AddCreateMethodStatement(Statement.ExpressionStatement(Expression.InvocationExpression(this.generatorContext.KnownSyntax.ChildLifecycleHandler.TryAddMethod, new Expression[] { targetIdentifier })));
                }
            }
        }

        return factoryNode with { DependeeArguments = ImmutableList.Create<Expression>(targetIdentifier) };
    }
}