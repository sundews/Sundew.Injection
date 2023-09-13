// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultValue.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

internal sealed record Literal(string Value) : CreationExpression(System.Array.Empty<Expression>())
{
    public static readonly Literal Null = new("null");
    public static readonly Literal False = new("false");
    public static readonly Literal True = new("true");
}