// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryDeclarations.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

internal class FactoryDeclarations(
    ClassDeclaration classNamespaceDeclaration,
    InterfaceDeclaration? interfaceNamespaceDeclaration,
    ValueArray<FactoryTargetDeclaration> createMethods)
{
    public ClassDeclaration ClassNamespaceDeclaration { get; } = classNamespaceDeclaration;

    public InterfaceDeclaration? InterfaceNamespaceDeclaration { get; } = interfaceNamespaceDeclaration;

    public ValueArray<FactoryTargetDeclaration> CreateMethods { get; } = createMethods;
}