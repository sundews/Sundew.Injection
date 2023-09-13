// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleInstancePerFactoryGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
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

internal class SingleInstancePerFactoryGenerator
{
    private const string Owned = "owned";
    private readonly GeneratorFeatures generatorFeatures;
    private readonly GeneratorContext generatorContext;

    public SingleInstancePerFactoryGenerator(
        GeneratorFeatures generatorFeatures,
        GeneratorContext generatorContext)
    {
        this.generatorFeatures = generatorFeatures;
        this.generatorContext = generatorContext;
    }

    public FactoryNode
       VisitSingleInstancePerFactory(
           SingleInstancePerFactoryInjectionNode singleInstancePerFactoryInjectionNode,
           in FactoryImplementation factoryImplementation,
           in MethodImplementation method)
    {
        var factoryNode = singleInstancePerFactoryInjectionNode.Parameters.Aggregate(
            new FactoryNode(in factoryImplementation, in method, ImmutableList<Expression>.Empty),
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

        var targetType = singleInstancePerFactoryInjectionNode.TargetType;
        var targetReferenceType = singleInstancePerFactoryInjectionNode.TargetReferenceType;
        var fieldType = singleInstancePerFactoryInjectionNode.ParameterNodeOption.GetValueOrDefault(x => x.Type, targetReferenceType);
        var (fields, wasAdded, targetTypeFieldDeclaration) = factoryNode.FactoryImplementation.Fields.GetOrAdd(
            NameHelper.GetIdentifierNameForType(targetType),
            fieldType,
            (fieldName) => new FieldDeclaration(fieldType, fieldName, null));

        var statements = factoryNode.FactoryImplementation.Constructor.Statements;

        var targetMemberAccessExpression = new MemberAccessExpression(Identifier.This, targetTypeFieldDeclaration.Name);
        var constructorParameters = factoryNode.FactoryImplementation.Constructor.Parameters;
        var dependeeArguments = ImmutableList.Create<Expression>(targetMemberAccessExpression);
        var factoryMethods = factoryNode.FactoryImplementation.FactoryMethods;
        if (wasAdded)
        {
            (factoryMethods, var creationExpression, factoryNode) = this.generatorFeatures.OptionalOverridableCreationGenerator.Generate(singleInstancePerFactoryInjectionNode, in factoryNode);
            if (singleInstancePerFactoryInjectionNode.ParameterNodeOption.TryGetValue(out var parameterNode))
            {
                (constructorParameters, _, var parameter, var argument, var needsFieldAssignment) = ParameterHelper.VisitParameter(
                    parameterNode,
                    NameHelper.GetIdentifierNameForType(parameterNode.Type),
                    constructorParameters,
                    ImmutableList<ParameterDeclaration>.Empty,
                    this.generatorContext.CompilationData);

                var (newFields, _, parameterField) = fields.GetOrAdd(
                    parameter.Name,
                    parameter.Type,
                    (fieldName) => new FieldDeclaration(parameter.Type, fieldName, null));
                fields = newFields;

                if (needsFieldAssignment)
                {
                    var optionalParameterFieldAssignment =
                        new ExpressionStatement(new AssignmentExpression(
                            new MemberAccessExpression(Identifier.This, parameterField.Name),
                            new Identifier(parameter.Name)));
                    statements = statements.Add(optionalParameterFieldAssignment);
                }

                var localDeclarationStatement = new LocalDeclarationStatement(Owned + targetType.Name, creationExpression);
                var localDeclarationIdentifier = new Identifier(localDeclarationStatement.Name);
                var localDeclarationAssignmentStatement = new ExpressionStatement(new AssignmentExpression(targetMemberAccessExpression, localDeclarationIdentifier));
                var trueStatements = ImmutableList.Create<Statement>(localDeclarationStatement);
                if (singleInstancePerFactoryInjectionNode.NeedsLifecycleHandling)
                {
                    var addInvocationStatement = new ExpressionStatement(new InvocationExpression(this.generatorContext.KnownSyntax.SharedLifecycleHandler.TryAddMethod, new Expression[] { localDeclarationIdentifier }));
                    trueStatements = trueStatements.Add(addInvocationStatement);
                }

                trueStatements = trueStatements.Add(localDeclarationAssignmentStatement);

                var falseStatements = ImmutableList.Create<Statement>(new ExpressionStatement(new AssignmentExpression(targetMemberAccessExpression, argument)));

                statements = statements.Add(new CreateOptionalParameterIfStatement(argument, trueStatements, falseStatements));
            }
            else
            {
                var assignmentStatement =
                    new ExpressionStatement(new AssignmentExpression(targetMemberAccessExpression, creationExpression));
                statements = statements.Add(assignmentStatement);

                if (singleInstancePerFactoryInjectionNode.NeedsLifecycleHandling)
                {
                    statements = statements.Add(new ExpressionStatement(new InvocationExpression(
                        this.generatorContext.KnownSyntax.SharedLifecycleHandler.TryAddMethod,
                        new Expression[] { targetMemberAccessExpression })));
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
                    Parameters = constructorParameters,
                },
                FactoryMethods = factoryMethods,
            },
            DependeeArguments = dependeeArguments,
        };
    }
}