// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeResolverSyntaxGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.TypeResolver.CodeGenerationStage;

using System;
using System.Collections.Immutable;
using System.Linq;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.Features.TypeResolver.ResolveGraphStage;
using Sundew.Injection.Generator.TypeSystem;

internal static class TypeResolverSyntaxGenerator
{
    private const string GetService = "GetService";
    private const string Type = "type";
    private const string BucketSize = "BucketSize";
    private const string ResolverItems = "resolverItems";
    private const string CreateName = "Create";

    public static ClassDeclaration Generate(ResolvedTypeResolverDefinition resolvedTypeResolverDefinition, CompilationData compilationData)
    {
        var (constructor, registrationCount) = CreateConstructor(resolvedTypeResolverDefinition, compilationData);

        return new ClassDeclaration(
            resolvedTypeResolverDefinition.ResolverType,
            true,
            new Member[]
            {
                Member._Field(new FieldDeclaration(compilationData.IntType, BucketSize, FieldModifier.Const, CreationExpression.Literal(GetBucketSize(registrationCount).ToString()))),
                Member._Field(new FieldDeclaration(compilationData.ResolverItemArrayType, ResolverItems, FieldModifier.Instance, null)),
                constructor,
                CreateResolveMethod(compilationData),
            },
            Array.Empty<AttributeDeclaration>(),
            ImmutableArray.Create(compilationData.ServiceProviderType));
    }

    private static int GetBucketSize(int length)
    {
        return (length * 3) switch
        {
            <= 13 => 13,
            <= 41 => 41,
            <= 127 => 127,
            <= 353 => 353,
            <= 1061 => 1061,
            <= 3163 => 3163,
            <= 9479 => 9479,
            <= 28433 => 28433,
            <= 42649 => 42649,
            <= 63977 => 63977,
            <= 95957 => 95957,
            _ => throw new NotSupportedException("Not Supported"),
        };
    }

    private static (Member Constructor, int BucketSize) CreateConstructor(ResolvedTypeResolverDefinition resolvedTypeResolverDefinition, CompilationData compilationData)
    {
        var factoryRegistrationWithParameters = resolvedTypeResolverDefinition.FactoryRegistrations
            .Select(x => (ParameterDeclaration: new ParameterDeclaration(x.FactoryType, x.FactoryType.Name.Uncapitalize()), FactoryRegistration: x)).ToArray();
        var supportedFactoryMethods = factoryRegistrationWithParameters
            .SelectMany(
                x => x.FactoryRegistration.FactoryMethods,
                (tuple, method) => (tuple.ParameterDeclaration, FactoryMethod: method))
            .GroupBy(
                y => y.FactoryMethod.ReturnType,
                x => x,
                (type, tuples) => (ReturnType: type, FactoryMethods: tuples.ToArray()))
            .Where(x => x.FactoryMethods.All(x => x.FactoryMethod.Parameters.IsEmpty)).ToArray();

        return (Member._MethodImplementation(
            new MethodDeclaration(
                DeclaredAccessibility.Public,
                false,
                false,
                resolvedTypeResolverDefinition.ResolverType.Name,
                factoryRegistrationWithParameters.Select(x => x.ParameterDeclaration).ToValueList(),
                ValueList<AttributeDeclaration>.Empty),
            new[]
            {
                Statement.ExpressionStatement(
                    Expression.AssignmentExpression(
                        Expression.MemberAccessExpression(Identifier.This, ResolverItems),
                        Expression._StaticMethodCall(
                            compilationData.ResolverItemsFactoryType,
                            CreateName,
                            ValueArray<DefiniteTypeArgument>.Empty,
                            new[] { Expression.Identifier(BucketSize) }
                                .Concat(
                                    supportedFactoryMethods.Select(factoryMethod =>
                                    {
                                        return Expression._ConstructorCall(
                                            compilationData.ResolverItemType,
                                            new[]
                                            {
                                                Expression.TypeOf(factoryMethod.ReturnType),
                                                Expression.Lambda(
                                                    Array.Empty<Expression>(),
                                                    CreateObjectExpression(factoryMethod.FactoryMethods, compilationData)),
                                            });
                                    })).ToValueArray()))),
            }), supportedFactoryMethods.Length);
    }

    private static Expression CreateObjectExpression(
        (ParameterDeclaration ParameterDeclaration, DefiniteFactoryMethod FactoryMethod)[] factoryMethodCalls,
        CompilationData compilationData)
    {
        var cardinality = factoryMethodCalls.ByCardinality();
        return cardinality switch
        {
            Single<(ParameterDeclaration ParameterDeclaration, DefiniteFactoryMethod FactoryMethod)> single =>
                Expression._InstanceMethodCall(
                    Expression.Identifier(single.Item.ParameterDeclaration.Name),
                    single.Item.FactoryMethod.Name,
                    ValueArray<DefiniteTypeArgument>.Empty,
                    Array.Empty<Expression>()),

            Multiple<(ParameterDeclaration ParameterDeclaration, DefiniteFactoryMethod FactoryMethod)> multiple =>
                CreationExpression._Array(
                    compilationData.ObjectType,
                    multiple.Select(x => Expression._InstanceMethodCall(
                        Expression.Identifier(x.ParameterDeclaration.Name),
                        x.FactoryMethod.Name,
                        ValueArray<DefiniteTypeArgument>.Empty,
                        Array.Empty<Expression>())).ToArray()),

            Empty<(ParameterDeclaration ParameterDeclaration, DefiniteFactoryMethod FactoryMethod)> empty => throw new System.NotImplementedException(),
        };
    }

    private static Member CreateResolveMethod(CompilationData compilationData)
    {
        return Member._Raw(
"""

        public object GetService(System.Type serviceType)
        {
            var index = global::System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(serviceType) % BucketSize;
            do
            {
                ref var item = ref this.resolverItems[index];
                if (ReferenceEquals(item.Type, serviceType))
                {
                    return item.Resolve.Invoke();
                }
            }
            while (index++ < BucketSize);

            throw new global::System.NotSupportedException($"The type: {serviceType} could not be found.");
        }                           
""");
    }
}
