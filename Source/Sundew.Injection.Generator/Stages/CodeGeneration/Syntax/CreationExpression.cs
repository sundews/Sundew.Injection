// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreationExpression.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

using System.Collections.Generic;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

[DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record CreationExpression(IReadOnlyList<Expression> Arguments) : InvocationExpressionBase(Arguments)
{
    public sealed record Array
        (Type ElementType, IReadOnlyList<Expression> Arguments) : CreationExpression(Arguments);

    public sealed record ConstructorCall
        (Type Type, IReadOnlyList<Expression> Arguments) : CreationExpression(Arguments);

    public sealed record StaticMethodCall
        (Type? Type, string Name, ValueArray<TypeArgument> TypeArguments, IReadOnlyList<Expression> Arguments) : CreationExpression(Arguments);

    public sealed record InstanceMethodCall
        (Expression FactoryAccessExpression, string Name, ValueArray<TypeArgument> TypeArguments, IReadOnlyList<Expression> Arguments) : CreationExpression(Arguments);

    public sealed record DefaultValue(Type Type) : CreationExpression(System.Array.Empty<Expression>());
}