// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryConstructorParameterGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;
using MethodImplementation = Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.MethodImplementation;

internal sealed class FactoryConstructorParameterGenerator
{
    private readonly InjectionNodeEvaluator injectionNodeEvaluator;
    private readonly CompilationData compilationData;
    private readonly KnownSyntax knownSyntax;

    public FactoryConstructorParameterGenerator(InjectionNodeEvaluator injectionNodeEvaluator, CompilationData compilationData, KnownSyntax knownSyntax)
    {
        this.injectionNodeEvaluator = injectionNodeEvaluator;
        this.compilationData = compilationData;
        this.knownSyntax = knownSyntax;
    }

    public FactoryNode
        VisitFactoryConstructorParameter(
            FactoryConstructorParameterInjectionNode factoryConstructorParameterInjectionNode,
            FactoryData factoryData,
            in FactoryImplementation factoryImplementation,
            in MethodImplementation method)
    {
        var factoryConstructorMethod = factoryImplementation.Constructor;
        var (parameters, wasAdded, parameter, argument) = ParameterHelper.VisitParameter(
            factoryConstructorParameterInjectionNode,
            null,
            factoryConstructorMethod.Parameters,
            ImmutableList<ParameterDeclaration>.Empty,
            true,
            false,
            this.compilationData);

        var identifier = new Identifier(parameter.Name);

        var fields = factoryImplementation.Fields;
        var statements = factoryImplementation.Constructor.Statements;
        if (wasAdded)
        {
            fields = fields.Add(new FieldDeclaration(parameter.Type, parameter.Name, null));

            var assignmentStatement =
                new ExpressionStatement(new AssignmentExpression(
                    new MemberAccessExpression(new Identifier("this"), parameter.Name), identifier));
            statements = statements.Add(assignmentStatement);
        }

        return new FactoryNode(
            factoryImplementation with
            {
                Fields = fields,
                Constructor = factoryImplementation.Constructor with
                {
                    Parameters = parameters,
                    Statements = statements,
                },
            },
            method,
            ImmutableList.Create(argument));
    }
}