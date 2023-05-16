// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Identifier.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

internal sealed record Identifier(string Name) : Expression
{
    public static readonly Identifier This = new Identifier("this");
}