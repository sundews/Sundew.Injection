// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethodHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System.Collections.Immutable;
using System.Linq;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.FactoryDataStage;
using Sundew.Injection.Generator.TypeSystem;
using MethodImplementation = Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.MethodImplementation;

internal static class FactoryMethodHelper
{
    public static (ImmutableList<DeclaredMethodImplementation> FactoryMethods, InvocationExpressionBase CreationExpression)
        GenerateFactoryMethod(
            ImmutableList<DeclaredMethodImplementation> factoryMethods, DefiniteType targetType, ValueArray<DefiniteParameter> parameters, CreationSource creationSource, ImmutableList<Expression> creationArguments)
    {
        var declaration = new MethodDeclaration(DeclaredAccessibility.Protected, true, "OnCreate" + NameHelper.GetFactoryMethodName(targetType.Name), parameters.Select(x => new ParameterDeclaration(x.Type, x.Name, null)).ToImmutableList(), targetType);
        var existingFactoryMethod = factoryMethods.FirstOrDefault(x => x.Declaration == declaration);
        if (Equals(existingFactoryMethod.Declaration, default))
        {
            factoryMethods = factoryMethods.Add(new DeclaredMethodImplementation(declaration, new MethodImplementation(declaration.Parameters, ImmutableList<Declaration>.Empty, ImmutableList.Create<Statement>(new ReturnStatement(new CreationExpression(creationSource, declaration.Parameters.Select(x => new Identifier(x.Name)).ToImmutableArray()))))));
        }

        return (factoryMethods, new InvocationExpression(new MemberAccessExpression(Identifier.This, declaration.Name), creationArguments));
    }
}