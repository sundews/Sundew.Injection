// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Expression.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

using System;
using System.Collections.Generic;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record Expression;

internal sealed record Lambda(IReadOnlyList<Expression> Parameters, Expression Expression) : Expression;

internal sealed record IndexerAccess(Expression Source, int Index) : Expression;

internal sealed record Identifier(string Name) : Expression
{
    public static readonly Identifier This = new("this");
}

internal sealed record FuncInvocationExpression(Expression DelegateAccessor, bool IsNullable) : Expression;

internal sealed record Cast(Expression Source, UsedType TargetType) : Expression;

internal sealed record AssignmentExpression(Expression Lhs, Expression Rhs) : Expression;

internal sealed record AwaitExpression(Expression Expression) : Expression;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record InvocationExpressionBase(IReadOnlyList<Expression> Arguments) : Expression;

internal sealed record InvocationExpression(Expression Expression, IReadOnlyList<Expression> Arguments) : InvocationExpressionBase(Arguments)
{
    public InvocationExpression(Expression expression)
        : this(expression, [])
    {
    }
}

[DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record CreationExpression(IReadOnlyList<Expression> Arguments) : InvocationExpressionBase(Arguments)
{
    public sealed record Array
        (Type ElementType, IReadOnlyList<Expression> Arguments) : CreationExpression(Arguments);

    public sealed record ConstructorCall
        (Type Type, IReadOnlyList<Expression> Arguments) : CreationExpression(Arguments);

    public sealed record StaticMethodCall
        (Type? Type, string Name, ValueArray<FullTypeArgument> TypeArguments, IReadOnlyList<Expression> Arguments) : CreationExpression(Arguments);

    public sealed record InstanceMethodCall
        (Expression FactoryAccessExpression, string Name, ValueArray<FullTypeArgument> TypeArguments, IReadOnlyList<Expression> Arguments) : CreationExpression(Arguments);

    public sealed record DefaultValue(Type Type) : CreationExpression(System.Array.Empty<Expression>());
}

internal sealed record Literal(string Value) : CreationExpression(System.Array.Empty<Expression>())
{
    public static readonly Literal Null = new("null");
    public static readonly Literal False = new("false");
    public static readonly Literal True = new("true");
}

internal sealed record NullCoalescingOperatorExpression(Expression Lhs, Expression Rhs, bool IsAssignment = false) : Expression;

internal sealed record TypeOf(Type Type) : Expression;

internal sealed record MemberAccessExpression(Expression Expression, string Name) : InvocationExpressionBase(Array.Empty<Expression>());