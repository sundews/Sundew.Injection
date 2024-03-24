// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryDeclarations.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

internal class FactoryDeclarations
{
    public FactoryDeclarations(ClassDeclaration classNamespaceDeclaration, InterfaceDeclaration? interfaceNamespaceDeclaration, ValueArray<DefiniteFactoryMethod> createMethods)
    {
        this.InterfaceNamespaceDeclaration = interfaceNamespaceDeclaration;
        this.CreateMethods = createMethods;
        this.ClassNamespaceDeclaration = classNamespaceDeclaration;
    }

    public ClassDeclaration ClassNamespaceDeclaration { get; }

    public InterfaceDeclaration? InterfaceNamespaceDeclaration { get; }

    public ValueArray<DefiniteFactoryMethod> CreateMethods { get; }
}