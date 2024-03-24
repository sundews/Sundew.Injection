// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberAccessExpression.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

using System;

internal sealed record MemberAccessExpression(Expression Expression, string Name) : InvocationExpressionBase(Array.Empty<Expression>());