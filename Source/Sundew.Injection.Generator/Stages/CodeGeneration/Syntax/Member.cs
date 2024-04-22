// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Member.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

using System.Collections.Generic;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record Member
{
    internal sealed record PropertyImplementation(PropertyDeclaration Declaration, IReadOnlyList<Statement> Statements) : Member;

    internal sealed record MethodImplementation(MethodDeclaration Declaration, IReadOnlyList<Statement> Statements) : Member;

    internal sealed record Field(FieldDeclaration Declaration) : Member;

    internal sealed record Raw(string Value) : Member;
}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record MemberDeclaration;