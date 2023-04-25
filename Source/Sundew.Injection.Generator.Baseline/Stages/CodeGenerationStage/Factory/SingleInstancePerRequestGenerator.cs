// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleInstancePerRequestGenerator.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System.Collections.Immutable;
using System.Linq;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;
using MethodImplementation = Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.MethodImplementation;

internal class SingleInstancePerRequestGenerator
{
    private readonly InjectionNodeEvaluator injectionNodeEvaluator;
    private readonly CompilationData compilationData;
    private readonly KnownSyntax knownSyntax;

    public SingleInstancePerRequestGenerator(InjectionNodeEvaluator injectionNodeEvaluator, CompilationData compilationData, KnownSyntax knownSyntax)
    {
        this.injectionNodeEvaluator = injectionNodeEvaluator;
        this.compilationData = compilationData;
        this.knownSyntax = knownSyntax;
    }

    public FactoryNode
        VisitSingleInstancePerRequest(
            SingleInstancePerRequestInjectionNode singleInstancePerRequestInjectionNode,
            FactoryData factoryModel,
            in FactoryImplementation factoryImplementation,
            in MethodImplementation method)
    {
        var factoryNode = singleInstancePerRequestInjectionNode.Parameters.Aggregate(
            new FactoryNode(in factoryImplementation, in method, ImmutableList<Expression>.Empty),
            (factoryNode, nextCreationNode) =>
            {
                var factory = factoryNode.FactoryImplementation;
                var factoryMethod = factoryNode.CreateMethod;
                var result = this.injectionNodeEvaluator.Evaluate(nextCreationNode, factoryModel, in factory, in factoryMethod);
                return factoryNode with
                {
                    FactoryImplementation = result.FactoryImplementation,
                    CreateMethod = result.CreateMethod,
                    Arguments = factoryNode.Arguments.AddRange(result.Arguments),
                };
            });

        var statements = factoryNode.CreateMethod.Statements;
        var targetType = singleInstancePerRequestInjectionNode.TargetType;
        var targetReferenceType = singleInstancePerRequestInjectionNode.TargetReferenceType;
        var variableType = singleInstancePerRequestInjectionNode.ParameterNodeOption.GetValueOrDefault(x => x.Type, targetReferenceType);
        var (variables, wasAdded, variableDeclaration) = factoryNode.CreateMethod.Variables.GetOrAddUnique(
            NameHelper.GetVariableNameForType(variableType),
            variableType,
            (conflictingName, i) => conflictingName + i,
            (name) => new Declaration(variableType, name));

        var targetIdentifier = new Identifier(variableDeclaration.Name);
        var factoryMethodParameters = factoryNode.CreateMethod.Parameters;
        var factoryMethods = factoryNode.FactoryImplementation.FactoryMethods;
        if (wasAdded)
        {
            (factoryMethods, var creationExpression) = singleInstancePerRequestInjectionNode.OverridableNewParametersOption.Evaluate(
                factoryMethods,
                (methodParameters, factoryMethods) => FactoryMethodHelper.GenerateFactoryMethod(factoryMethods, targetReferenceType, methodParameters, singleInstancePerRequestInjectionNode.CreationSource, factoryNode.Arguments),
                factoryMethods => (factoryMethods, new CreationExpression(singleInstancePerRequestInjectionNode.CreationSource, factoryNode.Arguments)));
            if (singleInstancePerRequestInjectionNode.ParameterNodeOption.HasValue)
            {
                var (parameterDeclarations, _, parameter, argument) = ParameterHelper.VisitParameter(
                    singleInstancePerRequestInjectionNode.ParameterNodeOption.Value,
                    variableDeclaration.Name,
                    factoryMethodParameters,
                    factoryImplementation.Constructor.Parameters,
                    false,
                    true,
                    this.compilationData);
                factoryMethodParameters = parameterDeclarations;

                var localDeclarationStatement = new LocalDeclarationStatement("owned" + targetType.Name, creationExpression);
                var localDeclarationIdentifier = new Identifier(localDeclarationStatement.Name);
                var localDeclarationAssignmentStatement = new ExpressionStatement(new AssignmentExpression(new Identifier(parameter.Name), localDeclarationIdentifier));
                var trueStatements = ImmutableList.Create<Statement>(localDeclarationStatement);
                if (singleInstancePerRequestInjectionNode.TargetImplementsDisposable)
                {
                    var addInvocationStatement = new ExpressionStatement(new InvocationExpression(this.knownSyntax.LocalDisposingListSyntax.AddMethod, new Expression[] { localDeclarationIdentifier }));
                    trueStatements = trueStatements.Add(addInvocationStatement);
                }

                trueStatements = trueStatements.Add(localDeclarationAssignmentStatement);
                statements = statements.Add(Statement.CreateOptionalParameterIfStatement(argument, trueStatements));
            }
            else
            {
                var assignmentStatement = new LocalDeclarationStatement(variableDeclaration.Name, creationExpression);
                statements = statements.Add(assignmentStatement);
                if (singleInstancePerRequestInjectionNode.TargetImplementsDisposable)
                {
                    statements = statements.Add(new ExpressionStatement(new InvocationExpression(this.knownSyntax.LocalDisposingListSyntax.AddMethod, new Expression[] { targetIdentifier })));
                }
            }
        }

        return factoryNode with
        {
            CreateMethod = factoryNode.CreateMethod with
            {
                Parameters = factoryMethodParameters,
                Variables = variables,
                Statements = statements,
                RequiresDisposingList = factoryNode.CreateMethod.RequiresDisposingList || singleInstancePerRequestInjectionNode.TargetImplementsDisposable,
            },
            FactoryImplementation = factoryNode.FactoryImplementation with { FactoryMethods = factoryMethods },
            Arguments = ImmutableList.Create<Expression>(targetIdentifier),
        };
    }
}