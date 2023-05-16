// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryDeclarations.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;

using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

internal class FactoryDeclarations
{
    public FactoryDeclarations(InterfaceDeclaration? interfaceNamespaceDeclaration, ClassDeclaration classNamespaceDeclaration)
    {
        this.InterfaceNamespaceDeclaration = interfaceNamespaceDeclaration;
        this.ClassNamespaceDeclaration = classNamespaceDeclaration;
    }

    public InterfaceDeclaration? InterfaceNamespaceDeclaration { get; }

    public ClassDeclaration ClassNamespaceDeclaration { get; }
}