// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Member.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

using System.Collections.Generic;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record Member;

internal sealed record MethodImplementation(MethodDeclaration MethodDeclaration, IReadOnlyList<Statement> Statements) : Member;

internal sealed record Field(FieldDeclaration Declaration) : Member;
