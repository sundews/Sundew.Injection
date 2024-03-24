// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodDeclaration.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

using System.Collections.Immutable;
using Sundew.Base.Collections.Immutable;

internal sealed record MethodDeclaration(DeclaredAccessibility Accessibility,
    bool IsVirtual,
    bool IsAsync,
    string Name,
    ValueList<ParameterDeclaration> Parameters,
    ValueList<AttributeDeclaration> Attributes,
    UsedType? ReturnType = null)
{
    public MethodDeclaration(
        DeclaredAccessibility accessibility,
        bool isVirtual,
        string name,
        ValueList<ParameterDeclaration> parameters,
        UsedType? returnType = null)
    : this(accessibility, isVirtual, false, name, parameters, ImmutableArray<AttributeDeclaration>.Empty, returnType)
    {
    }
}