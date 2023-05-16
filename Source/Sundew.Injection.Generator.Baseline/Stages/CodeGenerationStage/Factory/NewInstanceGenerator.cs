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
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;
using MethodImplementation = Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.MethodImplementation;

internal sealed class NewInstanceGenerator
{
    private readonly InjectionNodeEvaluator injectionNodeEvaluator;
    private readonly CompilationData compilationData;
    private readonly KnownSyntax knownSyntax;

    public NewInstanceGenerator(
        InjectionNodeEvaluator injectionNodeEvaluator,
        CompilationData compilationData,
        KnownSyntax knownSyntax)
    {
        this.injectionNodeEvaluator = injectionNodeEvaluator;
        this.compilationData = compilationData;
        this.knownSyntax = knownSyntax;
    }

    public FactoryNode
        VisitNewInstance(
            NewInstanceInjectionNode newInstanceInjectionNode,
            FactoryData factoryModel,
            in FactoryImplementation factoryImplementation,
            in MethodImplementation createMethod)
    {
        var factoryNode = newInstanceInjectionNode.Parameters.Aggregate(
            new FactoryNode(in factoryImplementation, in createMethod, ImmutableList<Expression>.Empty),
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

        var parentArguments = ImmutableList.Create<Expression>();
        var commonType = newInstanceInjectionNode.TargetReferenceType;

        var variables = factoryNode.CreateMethod.Variables;
        var statements = factoryNode.CreateMethod.Statements;
        var (factoryMethods, creationExpression) = newInstanceInjectionNode.OverridableNewParametersOption.Evaluate(
            factoryNode.FactoryImplementation.FactoryMethods,
            (methodParameters, factoryMethods) => FactoryMethodHelper.GenerateFactoryMethod(factoryMethods, commonType, methodParameters, newInstanceInjectionNode.CreationSource, factoryNode.Arguments),
            factoryMethods => (factoryMethods, new CreationExpression(newInstanceInjectionNode.CreationSource, factoryNode.Arguments)));

        var variableDeclarationOption = O.From(
            newInstanceInjectionNode.TargetImplementsDisposable || newInstanceInjectionNode.ParameterNodeOption.HasValue,
            () =>
            {
                var variableName = NameHelper.GetUniqueName(newInstanceInjectionNode.Name, newInstanceInjectionNode.ParentInjectionNode);
                return variables.GetOrAddUnique(
                    variableName,
                    commonType,
                    (s, i) => s + i,
                    name => new Declaration(commonType, name));
            });

        var factoryMethodParameters = factoryNode.CreateMethod.Parameters;
        if (newInstanceInjectionNode.ParameterNodeOption.HasValue)
        {
            var (parameterDeclarations, wasAdded, parameter, argument) = ParameterHelper.VisitParameter(
                newInstanceInjectionNode.ParameterNodeOption.Value,
                null,
                factoryMethodParameters,
                factoryImplementation.Constructor.Parameters,
                false,
                true,
                this.compilationData);
            var (newVariables, _, declaration) = variableDeclarationOption.Value;
            variables = newVariables;
            var localDeclarationStatement = new LocalDeclarationStatement(declaration.Name, new NullCoalescingOperatorExpression(argument, creationExpression));
            statements = statements.Add(localDeclarationStatement);
            var localDeclarationIdentifier = new Identifier(localDeclarationStatement.Name);
            if (newInstanceInjectionNode.TargetImplementsDisposable)
            {
                var addInvocationStatement = new ExpressionStatement(new InvocationExpression(this.knownSyntax.LocalDisposingListSyntax.AddMethod, new Expression[] { localDeclarationIdentifier }));
                statements = statements.Add(addInvocationStatement);
            }

            parentArguments = parentArguments.Add(localDeclarationIdentifier);
            factoryMethodParameters = parameterDeclarations;
        }
        else
        {
            if (variableDeclarationOption.HasValue)
            {
                var (newVariables, wasAdded, declaration) = variableDeclarationOption.Value;
                var variableIdentifier = new Identifier(declaration.Name);
                variables = newVariables;
                parentArguments = parentArguments.Add(variableIdentifier);
                if (wasAdded)
                {
                    statements = statements.Add(new LocalDeclarationStatement(variableIdentifier.Name, creationExpression))
                        .Add(new ExpressionStatement(new InvocationExpression(this.knownSyntax.LocalDisposingListSyntax.AddMethod, new Expression[] { variableIdentifier })));
                }
            }
            else
            {
                parentArguments = parentArguments.Add(creationExpression);
            }
        }

        return factoryNode with
        {
            CreateMethod = factoryNode.CreateMethod with
            {
                Variables = variables,
                Statements = statements,
                Parameters = factoryMethodParameters,
                RequiresDisposingList = factoryNode.CreateMethod.RequiresDisposingList || newInstanceInjectionNode.TargetImplementsDisposable,
            },
            Arguments = parentArguments,
            FactoryImplementation = factoryNode.FactoryImplementation with { FactoryMethods = factoryMethods },
        };
    }
}