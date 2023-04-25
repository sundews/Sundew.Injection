// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constructor.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;

using System.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

internal readonly record struct Constructor(
    ImmutableList<ParameterDeclaration> Parameters,
    ImmutableList<Statement> Statements,
    bool RequiresDisposableList);