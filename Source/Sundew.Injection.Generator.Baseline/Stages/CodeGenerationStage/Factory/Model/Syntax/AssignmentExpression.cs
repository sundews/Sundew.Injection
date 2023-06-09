﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssignmentExpression.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

internal sealed record AssignmentExpression(Expression Lhs, Expression Rhs) : Expression;