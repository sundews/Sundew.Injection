// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveTypeGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System.Collections.Generic;
using System.Collections.Immutable;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Primitives.Computation;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.TypeSystem;

internal class ResolveTypeGenerator
{
    private readonly GeneratorFeatures generatorFeatures;
    private readonly GeneratorContext generatorContext;

    public ResolveTypeGenerator(GeneratorFeatures generatorFeatures, GeneratorContext generatorContext)
    {
        this.generatorFeatures = generatorFeatures;
        this.generatorContext = generatorContext;
    }

    public (ImmutableList<Member.MethodImplementation> Methods, ImmutableList<Member.Field> Fields) Generate(
        NamedType factoryType, ImmutableList<DeclaredMethodImplementation> createMethods)
    {
        var field = new Member.Field(
            new FieldDeclaration(
            this.generatorContext.CompilationData.TypeResolver.ToDefiniteBoundGenericType(ImmutableArray.Create(new DefiniteTypeArgument(factoryType, new TypeMetadata(O.None, false, false, false)))),
            factoryType.FullName + "TypeResolver",
            true,
            this.GetCreationExpression(createMethods)));
        var resolveMethod = new Member.MethodImplementation(
            new MethodDeclaration(
                DeclaredAccessibility.Public,
                false,
                false,
                "Resolve",
                ImmutableList.Create(new ParameterDeclaration(this.generatorContext.CompilationData.TypeType, "type")).Add(new ParameterDeclaration(this.generatorContext.CompilationData.SpanOfObjectType, "arguments", "null")),
                ImmutableList<AttributeDeclaration>.Empty,
                this.generatorContext.CompilationData.ObjectType),
            this.GetStatements(field));
        return (ImmutableList.Create(resolveMethod), ImmutableList.Create(field));
    }

    private IReadOnlyList<Statement> GetStatements(Member.Field field)
    {
        return ImmutableArray.Create(new ReturnStatement(new InvocationExpression(
            new MemberAccessExpression(new MemberAccessExpression(Identifier.This, field.Declaration.Name), "Resolve"),
            new Expression[] { new Identifier("type"), new Identifier("arguments") })));
    }

    private CreationExpression GetCreationExpression(ImmutableList<DeclaredMethodImplementation> createMethods)
    {
        return null!;
    }
}