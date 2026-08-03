// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleInstancePerFactoryGenerator.cs" company="Sundews">
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
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
using Expression = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Expression;
using MethodImplementation = Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model.MethodImplementation;
using Statement = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Statement;

internal class SingleInstancePerFactoryGenerator(
    GeneratorFeatures generatorFeatures,
    GeneratorContext generatorContext)
{
    public FactoryNode
       VisitSingleInstancePerFactory(
           SingleInstancePerFactoryInjectionNode singleInstancePerFactoryInjectionNode,
           in FactoryImplementation factoryImplementation,
           in MethodImplementation method)
    {
        var factoryNode = new FactoryNode(in factoryImplementation, in method, ImmutableList<Expression>.Empty);

        var targetType = singleInstancePerFactoryInjectionNode.TargetType;
        var referencedType = singleInstancePerFactoryInjectionNode.ReferencedType;
        var (fieldType, fieldName) = singleInstancePerFactoryInjectionNode.ParameterNodeOption.GetValueOrDefault(x => (x.Type, NameHelper.GetIdentifierNameForType(targetType)), (referencedType, NameHelper.GetIdentifierNameForType(targetType)));

        (factoryNode, var wasAdded, var targetTypeFieldDeclaration) = factoryNode.GetOrAddField(
            fieldName,
            fieldType,
            (fieldName) => new FieldDeclaration(fieldType, fieldName, false, FieldModifier.Instance),
            (in FactoryNode factoryNode, bool willAdd, in FieldDeclaration _) =>
            {
                if (willAdd)
                {
                    return singleInstancePerFactoryInjectionNode.Parameters.Aggregate(
                        factoryNode,
                        (nextFactoryNode, nextInjectionNode) =>
                        {
                            var factory = nextFactoryNode.FactoryImplementation;
                            var factoryMethod = nextFactoryNode.RootFactoryMethod;
                            var result =
                                generatorFeatures.InjectionNodeExpressionGenerator.Generate(nextInjectionNode, in factory, in factoryMethod);
                            return nextFactoryNode with
                            {
                                FactoryImplementation = result.FactoryImplementation,
                                RootFactoryMethod = result.RootFactoryMethod,
                                DependantArguments =
                                nextFactoryNode.DependantArguments.AddRange(result.DependantArguments),
                            };
                        });
                }

                return factoryNode;
            });

        var targetMemberAccessExpression = new MemberAccessExpression(Identifier.This, targetTypeFieldDeclaration.Name);
        var dependeeArguments = ImmutableList.Create<Expression>(targetMemberAccessExpression);
        if (wasAdded)
        {
            (factoryNode, var creationExpression) = generatorFeatures.OptionalOverridableCreationGenerator.Generate(singleInstancePerFactoryInjectionNode, generatorContext.KnownSyntax.SharedLifecycleHandler, in factoryNode);
            if (singleInstancePerFactoryInjectionNode.ParameterNodeOption.TryGetValue(out var parameterNode))
            {
                var (parameterArgument, needsFieldAssignment) = ParameterHelper.VisitParameter(
                    parameterNode,
                    generatorContext.CompilationData);

                var isOptional = generatorContext.FactoryResolvedGraph.ReferencedTypesOptionality.TryGetValue(new RequestedParameter(parameterNode.ParameterSource.Type.Id, parameterNode.ParameterSource.Name), out var value) && value;

                (factoryNode, _, var parameterField) = factoryNode.GetOrAddField(
                    parameterNode.ParameterSource.Name,
                    parameterNode.ParameterSource.Type,
                    (fieldName) => new FieldDeclaration(parameterNode.ParameterSource.Type, fieldName, isOptional, FieldModifier.Instance));

                if (needsFieldAssignment)
                {
                    var optionalParameterFieldAssignment =
                        new ExpressionStatement(new AssignmentExpression(
                            new MemberAccessExpression(Identifier.This, parameterField.Name),
                            new Identifier(parameterNode.ParameterSource.Name)));
                    factoryNode = factoryNode.AddConstructorStatement(optionalParameterFieldAssignment);
                }

                if (singleInstancePerFactoryInjectionNode.Lifecycle != Lifecycle.None)
                {
                    creationExpression = new InvocationExpression(generatorContext.KnownSyntax.SharedLifecycleHandler.TryAddMethod, [creationExpression]);
                }

                var nullCoalescingOperatorExpression = Expression.NullCoalescingOperatorExpression(parameterArgument, creationExpression);
                factoryNode = factoryNode.AddConstructorStatement(Statement.ExpressionStatement(Expression.AssignmentExpression(targetMemberAccessExpression, nullCoalescingOperatorExpression)));
            }
            else
            {
                var assignmentStatement =
                    new ExpressionStatement(new AssignmentExpression(targetMemberAccessExpression, creationExpression));
                factoryNode = factoryNode.AddConstructorStatement(assignmentStatement);

                if (singleInstancePerFactoryInjectionNode.Lifecycle != Lifecycle.None)
                {
                    factoryNode = factoryNode.AddConstructorStatement(new ExpressionStatement(new InvocationExpression(
                        generatorContext.KnownSyntax.SharedLifecycleHandler.TryAddMethod,
                        [targetMemberAccessExpression])));
                }
            }
        }

        return factoryNode with { DependantArguments = dependeeArguments };
    }
}