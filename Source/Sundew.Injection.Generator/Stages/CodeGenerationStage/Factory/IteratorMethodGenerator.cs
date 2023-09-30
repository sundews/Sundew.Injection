// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IteratorMethodGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.FactoryDataStage;
using Sundew.Injection.Generator.TypeSystem;

internal sealed class IteratorMethodGenerator
{
    private const string Create = "Create";

    private readonly GeneratorFeatures generatorFeatures;
    private readonly GeneratorContext generatorContext;

    public IteratorMethodGenerator(GeneratorFeatures generatorFeatures, GeneratorContext generatorContext)
    {
        this.generatorFeatures = generatorFeatures;
        this.generatorContext = generatorContext;
    }

    public (FactoryNode FactoryNode, CreationExpression CreationExpression) Generate(CreationSource.IteratorMethodCall iteratorMethodCall, IReadOnlyList<Expression> arguments, in FactoryNode factoryNode)
    {
        var createMethodName = Create + NameHelper.GetFactoryMethodName(iteratorMethodCall.ElementType.Name);
        return (factoryNode with
            {
                CreateMethod = factoryNode.CreateMethod with
                {
                    Statements = factoryNode.CreateMethod.Statements.Add(
                        Statement.LocalFunctionStatement(
                            createMethodName,
                            ImmutableArray<ParameterDeclaration>.Empty,
                            iteratorMethodCall.ElementType,
                            arguments.Select(x => new YieldReturnStatement(x)).ToImmutableList<Statement>())),
                },
            },
            CreationExpression._StaticMethodCall(default, createMethodName, ImmutableArray<DefiniteTypeArgument>.Empty, Array.Empty<Expression>()));
    }
}