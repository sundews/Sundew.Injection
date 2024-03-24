// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclaredDisposeMethodImplementation.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;

using System.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Statement = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Statement;

internal readonly record struct DeclaredDisposeMethodImplementation(MethodDeclaration Declaration, ImmutableList<Statement> Statements);