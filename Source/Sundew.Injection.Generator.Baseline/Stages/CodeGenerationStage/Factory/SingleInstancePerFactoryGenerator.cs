// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleInstancePerFactoryGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System.Collections.Immutable;
using System.Linq;
using Sundew.Base;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;
using MethodImplementation = Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.MethodImplementation;

internal class SingleInstancePerFactoryGenerator
{
    private readonly InjectionNodeEvaluator injectionNodeEvaluator;
    private readonly CompilationData compilationData;
    private readonly KnownSyntax knownSyntax;

    public SingleInstancePerFactoryGenerator(InjectionNodeEvaluator injectionNodeEvaluator, CompilationData compilationData, KnownSyntax knownSyntax)
    {
        this.injectionNodeEvaluator = injectionNodeEvaluator;
        this.compilationData = compilationData;
        this.knownSyntax = knownSyntax;
    }

    public FactoryNode
       VisitSingleInstancePerFactory(
           SingleInstancePerFactoryInjectionNode singleInstancePerFactoryInjectionNode,
           FactoryData factoryModel,
           in FactoryImplementation factoryImplementation,
           in MethodImplementation method)
    {
        var factoryNode = singleInstancePerFactoryInjectionNode.Parameters.Aggregate(
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

        var targetType = singleInstancePerFactoryInjectionNode.TargetType;
        var targetReferenceType = singleInstancePerFactoryInjectionNode.TargetReferenceType;
        var fieldType = singleInstancePerFactoryInjectionNode.ParameterNodeOption.GetValueOrDefault(x => x.Type, targetReferenceType);
        var (fields, wasAdded, fieldDeclaration) = factoryNode.FactoryImplementation.Fields.GetOrAddUnique(
            NameHelper.GetVariableNameForType(targetType),
            fieldType,
            (conflictingName, i) => conflictingName + i,
            (fieldName) => new FieldDeclaration(fieldType, fieldName, null));

        var statements = factoryNode.FactoryImplementation.Constructor.Statements;

        var targetMemberAccessExpression = new MemberAccessExpression(Identifier.This, fieldDeclaration.Name);
        var constructorParameters = factoryNode.FactoryImplementation.Constructor.Parameters;
        var parameters = ImmutableList.Create<Expression>(targetMemberAccessExpression);
        var factoryMethods = factoryNode.FactoryImplementation.FactoryMethods;
        if (wasAdded)
        {
            (factoryMethods, var creationExpression) = singleInstancePerFactoryInjectionNode.OverridableNewParametersOption.GetValueOrDefault(
                factoryMethods,
                (methodParameters, factoryMethods) => FactoryMethodHelper.GenerateFactoryMethod(factoryMethods, targetReferenceType, methodParameters, singleInstancePerFactoryInjectionNode.CreationSource, factoryNode.Arguments),
                factoryMethods => (factoryMethods, new CreationExpression(singleInstancePerFactoryInjectionNode.CreationSource, factoryNode.Arguments)));

            if (singleInstancePerFactoryInjectionNode.ParameterNodeOption.TryGetValue(out var parameterNode))
            {
                var (parameterDeclarations, _, parameter, argument) = ParameterHelper.VisitParameter(
                    parameterNode,
                    null,
                    constructorParameters,
                    ImmutableList<ParameterDeclaration>.Empty,
                    true,
                    true,
                    this.compilationData);
                constructorParameters = parameterDeclarations;

                var (newFields, _, declaration) = fields.GetOrAddUnique(
                    NameHelper.GetVariableNameForType(parameter.Type),
                    parameter.Type,
                    (conflictingName, i) => conflictingName + i,
                    (fieldName) => new FieldDeclaration(parameter.Type, fieldName, null));
                fields = newFields;

                var optionalParameterFieldAssignment =
                    new ExpressionStatement(new AssignmentExpression(
                        new MemberAccessExpression(Identifier.This, parameter.Name), new Identifier(parameter.Name)));

                var localDeclarationStatement = new LocalDeclarationStatement("owned" + targetType.Name, creationExpression);
                var localDeclarationIdentifier = new Identifier(localDeclarationStatement.Name);
                var localDeclarationAssignmentStatement = new ExpressionStatement(new AssignmentExpression(targetMemberAccessExpression, localDeclarationIdentifier));
                var trueStatements = ImmutableList.Create<Statement>(localDeclarationStatement);
                if (singleInstancePerFactoryInjectionNode.TargetImplementsDisposable)
                {
                    var addInvocationStatement = new ExpressionStatement(new InvocationExpression(this.knownSyntax.FactoryConstructorDisposingListSyntax.AddMethod, new Expression[] { localDeclarationIdentifier }));
                    trueStatements = trueStatements.Add(addInvocationStatement);
                }

                statements = statements.Add(optionalParameterFieldAssignment);
                trueStatements = trueStatements.Add(localDeclarationAssignmentStatement);
                var falseStatements = ImmutableList.Create<Statement>(new ExpressionStatement(new AssignmentExpression(targetMemberAccessExpression, argument)));

                statements = statements.Add(new CreateOptionalParameterIfStatement(argument, trueStatements, falseStatements));
            }
            else
            {
                var assignmentStatement = new ExpressionStatement(new AssignmentExpression(targetMemberAccessExpression, creationExpression));
                statements = statements.Add(assignmentStatement);

                if (singleInstancePerFactoryInjectionNode.TargetImplementsDisposable)
                {
                    statements = statements.Add(new ExpressionStatement(new InvocationExpression(this.knownSyntax.FactoryConstructorDisposingListSyntax.AddMethod, new Expression[] { targetMemberAccessExpression })));
                }
            }
        }

        return factoryNode with
        {
            FactoryImplementation = factoryNode.FactoryImplementation with
            {
                Fields = fields,
                Constructor = factoryNode.FactoryImplementation.Constructor with
                {
                    Statements = statements,
                    RequiresDisposableList = factoryNode.FactoryImplementation.Constructor.RequiresDisposableList || singleInstancePerFactoryInjectionNode.TargetImplementsDisposable,
                    Parameters = constructorParameters,
                },
                FactoryMethods = factoryMethods,
            },
            Arguments = parameters,
        };
    }
}