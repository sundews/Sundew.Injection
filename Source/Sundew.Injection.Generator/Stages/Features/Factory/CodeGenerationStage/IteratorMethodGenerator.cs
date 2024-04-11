// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IteratorMethodGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;
using Sundew.Injection.Generator.TypeSystem;

internal sealed class IteratorMethodGenerator
{
    private const string Create = "Create";

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
                            iteratorMethodCall.ReturnType,
                            arguments.Select(x => new YieldReturnStatement(x)).ToImmutableList<Statement>(),
                            !arguments.Any(x => x is MemberAccessExpression))),
            },
        },
            CreationExpression._StaticMethodCall(default, createMethodName, ImmutableArray<TypeArgument>.Empty, []));
    }
}