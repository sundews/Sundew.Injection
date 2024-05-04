// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Statement.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

using System.Collections.Immutable;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record Statement;

internal sealed record ExpressionStatement(Expression Expression) : Statement;

internal sealed record ReturnStatement(Expression Expression) : Statement;

internal sealed record YieldReturnStatement(Expression Expression) : Statement;

internal sealed record LocalDeclarationStatement(string Name, Expression Initializer) : Statement;

internal sealed record CreateOptionalParameterIfStatement(Expression ConditionAccess, ImmutableList<Statement> TrueStatements, ImmutableList<Statement>? FalseStatements = null) : Statement;

internal sealed record LocalFunctionStatement(string Name, ValueList<ParameterDeclaration> Parameters, Type ReturnType, ImmutableList<Statement> Statements, bool IsStatic) : Statement;

internal sealed record Raw(string Source) : Statement;