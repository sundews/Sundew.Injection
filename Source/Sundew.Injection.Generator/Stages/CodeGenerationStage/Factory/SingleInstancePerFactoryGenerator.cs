// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleInstancePerFactoryGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
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
        var factoryNode = new FactoryNode(in factoryImplementation, in method, ImmutableList<Expression>.Empty);

        var targetType = singleInstancePerFactoryInjectionNode.TargetType;
        var targetReferenceType = singleInstancePerFactoryInjectionNode.TargetReferenceType;
        var fieldType = singleInstancePerFactoryInjectionNode.ParameterNodeOption.GetValueOrDefault(x => x.Type, targetReferenceType);

        (factoryNode, var wasAdded, var targetTypeFieldDeclaration) = factoryNode.GetOrAddField(
            NameHelper.GetIdentifierNameForType(targetType),
            fieldType,
            (fieldName) => new FieldDeclaration(fieldType, fieldName, false, null),
            (in FactoryNode factoryNode, bool willAdd, in FieldDeclaration _) =>
            {
                if (willAdd)
                {
                    return singleInstancePerFactoryInjectionNode.Parameters.Aggregate(
                        factoryNode,
                        (nextFactoryNode, nextInjectionNode) =>
                        {
                            var factory = nextFactoryNode.FactoryImplementation;
                            var factoryMethod = nextFactoryNode.CreateMethod;
                            var result =
                                this.generatorFeatures.InjectionNodeExpressionGenerator.Generate(nextInjectionNode, in factory, in factoryMethod);
                            return nextFactoryNode with
                            {
                                FactoryImplementation = result.FactoryImplementation,
                                CreateMethod = result.CreateMethod,
                                DependeeArguments =
                                nextFactoryNode.DependeeArguments.AddRange(result.DependeeArguments),
                            };
                        });
                }

                return factoryNode;
            });

        var targetMemberAccessExpression = new MemberAccessExpression(Identifier.This, targetTypeFieldDeclaration.Name);
        var dependeeArguments = ImmutableList.Create<Expression>(targetMemberAccessExpression);
        if (wasAdded)
        {
            (factoryNode, var creationExpression) = this.generatorFeatures.OptionalOverridableCreationGenerator.Generate(singleInstancePerFactoryInjectionNode, in factoryNode);
            if (singleInstancePerFactoryInjectionNode.ParameterNodeOption.TryGetValue(out var parameterNode))
            {
                (factoryNode, _, var parameter, var argument, var needsFieldAssignment) = factoryNode.GetOrAddConstructorParameter(
                    parameterNode,
                    NameHelper.GetIdentifierNameForType(parameterNode.Type),
                    ImmutableList<ParameterDeclaration>.Empty,
                    this.generatorContext.CompilationData);

                (factoryNode, _, var parameterField) = factoryNode.GetOrAddField(
                    parameter.Name,
                    parameter.Type,
                    (fieldName) => new FieldDeclaration(parameter.Type, fieldName, false, null));

                if (needsFieldAssignment)
                {
                    var optionalParameterFieldAssignment =
                        new ExpressionStatement(new AssignmentExpression(
                            new MemberAccessExpression(Identifier.This, parameterField.Name),
                            new Identifier(parameter.Name)));
                    factoryNode = factoryNode.AddConstructorStatement(optionalParameterFieldAssignment);
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

                factoryNode = factoryNode.AddConstructorStatement(new CreateOptionalParameterIfStatement(argument, trueStatements, falseStatements));
            }
            else
            {
                var assignmentStatement =
                    new ExpressionStatement(new AssignmentExpression(targetMemberAccessExpression, creationExpression));
                factoryNode = factoryNode.AddConstructorStatement(assignmentStatement);

                if (singleInstancePerFactoryInjectionNode.NeedsLifecycleHandling)
                {
                    factoryNode = factoryNode.AddConstructorStatement(new ExpressionStatement(new InvocationExpression(
                        this.generatorContext.KnownSyntax.SharedLifecycleHandler.TryAddMethod,
                        new Expression[] { targetMemberAccessExpression })));
                }
            }
        }

        return factoryNode with { DependeeArguments = dependeeArguments };
    }
}