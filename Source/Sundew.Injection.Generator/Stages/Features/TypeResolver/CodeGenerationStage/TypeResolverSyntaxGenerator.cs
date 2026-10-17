// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeResolverSyntaxGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.TypeResolver.CodeGenerationStage;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.CodeGeneration.Templates;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.Features.TypeResolver.ResolveGraphStage;
using Sundew.Injection.Generator.TypeSystem;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

internal static class TypeResolverSyntaxGenerator
{
    private const string BucketSize = "BucketSize";
    private const string ServiceResolver = "serviceResolver";
    private const string ArgumentsName = "arguments";
    private const string InstanceName = "instance";
    private const string ActualInstanceName = "actualInstance";
    private const string DisposeName = "Dispose";
    private const string DisposeAsyncName = "DisposeAsync";

    public static ClassDeclaration Generate(ResolvedTypeResolverDefinition resolvedTypeResolverDefinition, CompilationData compilationData)
    {
        var (constructor, registrationCount) = CreateConstructor(resolvedTypeResolverDefinition, compilationData);

        return new ClassDeclaration(
            resolvedTypeResolverDefinition.ResolverType,
            true,
            [
                Member._Field(new FieldDeclaration(compilationData.IntType, BucketSize, false, FieldModifier.Const, CreationExpression.Literal(GetBucketSize(registrationCount).ToString()))),
                Member._Field(new FieldDeclaration(compilationData.ProvidedSundewInjectionCompilationData.ServiceResolverType, ServiceResolver, false, FieldModifier.Instance)),
                constructor,
                CreateServiceProviderMembers(),
            ],
            [],
            ImmutableArray.Create<Type>(compilationData.ReferencedSundewInjectionCompilationData.IInjectionServiceProviderType));
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

    private static (Member Constructor, int RegistrationCount) CreateConstructor(ResolvedTypeResolverDefinition resolvedTypeResolverDefinition, CompilationData compilationData)
    {
        var factoryRegistrationWithParameters = resolvedTypeResolverDefinition.FactoryRegistrations
            .Select(x => (ParameterDeclaration: new ParameterDeclaration(x.FactoryType, x.FactoryType.Name.Uncapitalize(), ParameterNecessity._Required), FactoryRegistration: x)).ToArray();
        var registrations = factoryRegistrationWithParameters
            .SelectMany(
                x => x.FactoryRegistration.FactoryMethods,
                (tuple, method) => (tuple.ParameterDeclaration, FactoryMethod: method))
            .SelectMany(x => GetCallableParameterCounts(x.FactoryMethod).Select(parameterCount => new FactoryMethodCall(x.ParameterDeclaration, x.FactoryMethod, parameterCount)))
            .GroupBy(
                x => x.FactoryMethod.ReturnType,
                x => x,
                (type, factoryMethodCalls) => (ReturnType: type, FactoryMethodCalls: factoryMethodCalls.ToArray()))
            .ToArray();

        return (Member._MethodImplementation(
            new MethodDeclaration(
                DeclaredAccessibility.Public,
                false,
                false,
                false,
                false,
                resolvedTypeResolverDefinition.ResolverType.Name,
                factoryRegistrationWithParameters.Select(x => x.ParameterDeclaration).ToValueList(),
                ValueList<AttributeDeclaration>.Empty),
            [
                Statement.ExpressionStatement(
                    Expression.AssignmentExpression(
                        Expression.MemberAccessExpression(Identifier.This, ServiceResolver),
                        Expression._ConstructorCall(
                            compilationData.ProvidedSundewInjectionCompilationData.ServiceResolverType,
                            new[] { Expression.Identifier(BucketSize) }
                                .Concat(registrations.Select(registration => CreateResolverItem(registration.ReturnType, registration.FactoryMethodCalls, compilationData)))
                                .ToValueArray()))),
            ]), registrations.Length);
    }

    private static IEnumerable<int> GetCallableParameterCounts(FactoryTargetDeclaration factoryMethod)
    {
        if (factoryMethod.IsProperty)
        {
            return [0];
        }

        var parameters = factoryMethod.Parameters;
        var omittableParameterCount = parameters.Reverse().TakeWhile(x => x.ParameterNecessity is ParameterNecessity.Optional { HasDefaultValue: true }).Count();
        return Enumerable.Range(parameters.Count - omittableParameterCount, omittableParameterCount + 1);
    }

    private static Expression CreateResolverItem(Type returnType, FactoryMethodCall[] factoryMethodCalls, CompilationData compilationData)
    {
        var parameterlessCalls = factoryMethodCalls.Where(x => x.ParameterCount == 0).ToArray();
        var resolveExpression = parameterlessCalls.Length > 0
            ? Expression.Lambda([], CreateParameterlessObjectExpression(parameterlessCalls, compilationData))
            : Literal.Null;

        var seenSignatures = new HashSet<ValueArray<Type>>();
        var argumentResolvers = factoryMethodCalls
            .Where(x => x.ParameterCount > 0 && seenSignatures.Add(x.FactoryMethod.Parameters.Take(x.ParameterCount).Select(parameter => parameter.Type).ToValueArray()))
            .Select(x => CreateArgumentResolver(x, compilationData));

        var disposingFactories = factoryMethodCalls
            .Where(x => x.FactoryMethod.HasDispose || x.FactoryMethod.HasDisposeAsync)
            .GroupBy(
                x => x.ParameterDeclaration.Name,
                x => x.FactoryMethod,
                (name, factoryMethods) => new DisposingFactory(name, factoryMethods.Any(x => x.HasDispose), factoryMethods.Any(x => x.HasDisposeAsync)))
            .ToArray();
        var disposeExpressions = disposingFactories.Length > 0
            ? CreateTryDisposeExpressions(returnType, disposingFactories)
            : [];

        return Expression._ConstructorCall(
            compilationData.ProvidedSundewInjectionCompilationData.ResolverItemType,
            new[] { Expression.TypeOf(returnType), resolveExpression }.Concat(disposeExpressions).Concat(argumentResolvers).ToArray());
    }

    private static Expression[] CreateTryDisposeExpressions(Type returnType, DisposingFactory[] disposingFactories)
    {
        const string valueTaskOfBool = "global::System.Threading.Tasks.ValueTask<bool>";
        var isCheck = $"if ({InstanceName} is {new StringBuilder().AppendFullyQualifiedType(returnType)} {ActualInstanceName})";
        var disposeStatements = disposingFactories.Select(x => x.HasDispose
            ? $"{x.FactoryName}.{DisposeName}({ActualInstanceName});"
            : $"{x.FactoryName}.{DisposeAsyncName}({ActualInstanceName}).AsTask().GetAwaiter().GetResult();");
        var tryDispose = CreateTryDisposeLambda(string.Empty, isCheck, disposeStatements, "true", "false");

        var tryDisposeAsync = disposingFactories.Any(x => x.HasDisposeAsync)
            ? CreateTryDisposeLambda(
                "async ",
                isCheck,
                disposingFactories.Select(x => x.HasDisposeAsync
                    ? $"await {x.FactoryName}.{DisposeAsyncName}({ActualInstanceName}).ConfigureAwait(false);"
                    : $"{x.FactoryName}.{DisposeName}({ActualInstanceName});"),
                "true",
                "false")
            : CreateTryDisposeLambda(string.Empty, isCheck, disposeStatements, $"new {valueTaskOfBool}(true)", $"new {valueTaskOfBool}(false)");

        return [Expression.Identifier(tryDispose), Expression.Identifier(tryDisposeAsync)];
    }

    private static string CreateTryDisposeLambda(string modifier, string isCheck, IEnumerable<string> disposeStatements, string disposedResult, string notDisposedResult)
    {
        const string indentation = "                    ";
        var newLine = Environment.NewLine;
        return new StringBuilder()
            .Append(modifier).Append('(').Append(InstanceName).Append(") =>").Append(newLine)
            .Append(indentation).Append('{').Append(newLine)
            .Append(indentation).Append("    ").Append(isCheck).Append(newLine)
            .Append(indentation).Append("    {").Append(newLine)
            .AppendItems(disposeStatements, (builder, statement) => builder.Append(indentation).Append("        ").Append(statement).Append(newLine))
            .Append(indentation).Append("        return ").Append(disposedResult).Append(';').Append(newLine)
            .Append(indentation).Append("    }").Append(newLine)
            .Append(newLine)
            .Append(indentation).Append("    return ").Append(notDisposedResult).Append(';').Append(newLine)
            .Append(indentation).Append('}')
            .ToString();
    }

    private static Expression CreateArgumentResolver(FactoryMethodCall factoryMethodCall, CompilationData compilationData)
    {
        var parameters = factoryMethodCall.FactoryMethod.Parameters.Take(factoryMethodCall.ParameterCount).ToArray();
        return Expression._ConstructorCall(
            compilationData.ProvidedSundewInjectionCompilationData.ArgumentResolverType,
            [
                CreationExpression._Array(compilationData.TypeType, parameters.Select(x => Expression.TypeOf(x.Type)).ToArray()),
                Expression.Lambda(
                    [Expression.Identifier(ArgumentsName)],
                    Expression._InstanceMethodCall(
                        Expression.Identifier(factoryMethodCall.ParameterDeclaration.Name),
                        factoryMethodCall.FactoryMethod.Name,
                        ValueArray<FullTypeArgument>.Empty,
                        parameters.Select((parameter, index) => Expression.Cast(Expression.Identifier($"{ArgumentsName}[{index}]!"), new UsedType(parameter.Type))).ToArray())),
            ]);
    }

    private static Expression CreateParameterlessObjectExpression(
        FactoryMethodCall[] factoryMethodCalls,
        CompilationData compilationData)
    {
        var cardinality = factoryMethodCalls.ByCardinality();
        return cardinality switch
        {
            Single<FactoryMethodCall> single => CreateParameterlessCall(single.Item),
            Multiple<FactoryMethodCall> multiple => CreationExpression._Array(
                compilationData.ObjectType,
                multiple.Select(CreateParameterlessCall).ToArray()),
            Empty<FactoryMethodCall> => throw new NotSupportedException(),
        };
    }

    private static Expression CreateParameterlessCall(FactoryMethodCall factoryMethodCall)
    {
        return factoryMethodCall.FactoryMethod.IsProperty
            ? Expression.MemberAccessExpression(
                Expression.Identifier(factoryMethodCall.ParameterDeclaration.Name),
                factoryMethodCall.FactoryMethod.Name)
            : Expression._InstanceMethodCall(
                Expression.Identifier(factoryMethodCall.ParameterDeclaration.Name),
                factoryMethodCall.FactoryMethod.Name,
                ValueArray<FullTypeArgument>.Empty,
                []);
    }

    private static Member CreateServiceProviderMembers()
    {
        return Member._Raw(
"""

        public object? GetService(global::System.Type serviceType)
        {
            return this.serviceResolver.TryGetService(serviceType, out var service, default) ? service : null;
        }

        public bool TryGetService(global::System.Type serviceType, out object? service, params global::System.ReadOnlySpan<object?> arguments)
        {
            return this.serviceResolver.TryGetService(serviceType, out service, arguments);
        }

        public bool TryDispose(object instance)
        {
            return this.serviceResolver.TryDispose(instance);
        }

        public global::System.Threading.Tasks.ValueTask<bool> TryDisposeAsync(object instance)
        {
            return this.serviceResolver.TryDisposeAsync(instance);
        }
""");
    }

    private readonly record struct DisposingFactory(string FactoryName, bool HasDispose, bool HasDisposeAsync);

    private readonly record struct FactoryMethodCall(ParameterDeclaration ParameterDeclaration, FactoryTargetDeclaration FactoryMethod, int ParameterCount);
}
