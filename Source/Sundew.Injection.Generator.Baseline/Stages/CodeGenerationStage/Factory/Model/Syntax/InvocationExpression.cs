// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvocationExpression.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

using System;
using System.Collections.Generic;

internal sealed record InvocationExpression(Expression Expression, IReadOnlyList<Expression> Arguments) : InvocationExpressionBase(Arguments)
{
    public InvocationExpression(Expression expression)
    : this(expression, Array.Empty<Expression>())
    {
    }
}