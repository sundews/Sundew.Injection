// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveTypeGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Primitives.Computation;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Templates;
using Sundew.Injection.Generator.TypeSystem;

internal class ResolveTypeGenerator
{
    private const string Arguments = "arguments";
    private const string Resolve = "Resolve";
    private const string Type = "type";
    private const string TypeResolver = "TypeResolver";
    private static readonly Expression FactoryIdentifier = Expression.Identifier("factory");
    private static readonly Expression ArgumentsIdentifier = Expression.Identifier(Arguments);
    private readonly GeneratorFeatures generatorFeatures;
    private readonly GeneratorContext generatorContext;

    public ResolveTypeGenerator(GeneratorFeatures generatorFeatures, GeneratorContext generatorContext)
    {
        this.generatorFeatures = generatorFeatures;
        this.generatorContext = generatorContext;
    }

    public (ImmutableList<Member.MethodImplementation> Methods, ImmutableList<Member.Field> Fields) Generate(
        NamedType factoryType, ValueList<CreateMethod> createMethods)
    {
        var factoryTypeTypeArguments = ImmutableArray.Create(new DefiniteTypeArgument(factoryType, new TypeMetadata(O.None, false, false)));
        var typeResolver = createMethods.Count > 100
            ? this.generatorContext.CompilationData.TypeResolverBinarySearch
            : this.generatorContext.CompilationData.TypeResolverLinearSearch;
        var typeResolverType = typeResolver.ToDefiniteBoundGenericType(
            factoryTypeTypeArguments);
        var creationExpression = this.GetCreationExpression(createMethods, typeResolverType, this.generatorContext.CompilationData.Resolver.ToDefiniteBoundGenericType(factoryTypeTypeArguments));

        var field = new Member.Field(
            new FieldDeclaration(
                typeResolverType,
                factoryType.Name + TypeResolver,
                true,
                creationExpression));
        var resolveMethod = new Member.MethodImplementation(
            new MethodDeclaration(
                DeclaredAccessibility.Public,
                false,
                false,
                Resolve,
                ImmutableList.Create(new ParameterDeclaration(this.generatorContext.CompilationData.TypeType, Type)).Add(new ParameterDeclaration(this.generatorContext.CompilationData.SpanOfObjectType, Arguments, Trivia.Default)),
                ImmutableList<AttributeDeclaration>.Empty,
                new UsedType(this.generatorContext.CompilationData.ObjectType, true)),
            this.GetStatements(field));
        return (ImmutableList.Create(resolveMethod), ImmutableList.Create(field)).ToSuccess();
    }

    private IReadOnlyList<Statement> GetStatements(Member.Field field)
    {
        return ImmutableArray.Create(new ReturnStatement(new InvocationExpression(
            new MemberAccessExpression(new Identifier(field.Declaration.Name), Resolve),
            new Expression[] { Identifier.This, new Identifier(Type), new Identifier(Arguments) })));
    }

    private CreationExpression GetCreationExpression(
        ValueList<CreateMethod> createMethods,
        DefiniteClosedGenericType factoryTypeResolverType,
        DefiniteClosedGenericType resolverType)
    {
        var resolveTypeInitialization = createMethods.Select(x => Expression._ConstructorCall(resolverType, ImmutableArray.Create(Expression.TypeOf(x.ReturnType), this.GetFactoryLambda(x))));

        return CreationExpression._ConstructorCall(factoryTypeResolverType, resolveTypeInitialization.ToArray()).ToSuccess();
    }

    private Expression GetFactoryLambda(CreateMethod createMethod)
    {
        return Expression.Lambda(
            ImmutableArray.Create(FactoryIdentifier, ArgumentsIdentifier),
            Expression._InstanceMethodCall(FactoryIdentifier, createMethod.Name, ImmutableArray<DefiniteTypeArgument>.Empty, createMethod.Parameters.Select((x, index) => Expression.Cast(Expression.IndexerAccess(ArgumentsIdentifier, index), new UsedType(x.Type, x.DefaultValue != null))).ToArray()));
    }
}