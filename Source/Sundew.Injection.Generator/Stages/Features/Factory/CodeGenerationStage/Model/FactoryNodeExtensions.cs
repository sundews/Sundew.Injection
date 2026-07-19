// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryNodeExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;

using System;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Statement = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Statement;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

internal delegate FactoryNode ModifyFactoryNode<TDeclaration>(in FactoryNode factoryNode, bool willAdd, in TDeclaration declaration)
    where TDeclaration : struct;

internal static class FactoryNodeExtensions
{
    public static (FactoryNode FactoryNode, bool WasAdded, FieldDeclaration FieldDeclaration) GetOrAddField(
        in this FactoryNode factoryNode,
        string name,
        Type type,
        Func<string, FieldDeclaration> createDeclarationFunc,
        ModifyFactoryNode<FieldDeclaration>? preModifyFactoryNodeFunc = null)
    {
        var (wasCreated, fieldDeclaration) = factoryNode.FactoryImplementation.Fields.GetOrCreate(
            name,
            type,
            createDeclarationFunc);
        var modifiedFactoryNode = preModifyFactoryNodeFunc?.Invoke(factoryNode, wasCreated, fieldDeclaration) ?? factoryNode;
        var fields = wasCreated ? modifiedFactoryNode.FactoryImplementation.Fields.Add(fieldDeclaration) : modifiedFactoryNode.FactoryImplementation.Fields;
        return (modifiedFactoryNode with { FactoryImplementation = modifiedFactoryNode.FactoryImplementation with { Fields = fields } }, wasCreated, fieldDeclaration);
    }

    public static (FactoryNode FactoryNode, bool WasAdded, Declaration Declaration) GetOrAddVariable(
        in this FactoryNode factoryNode,
        string name,
        Type type,
        Func<string, Declaration> createDeclarationFunc,
        ModifyFactoryNode<Declaration>? preModifyFactoryNodeFunc = null)
    {
        var (wasCreated, declaration) = factoryNode.RootFactoryMethod.Variables.GetOrCreate(
            name,
            type,
            createDeclarationFunc);
        var modifiedFactoryNode = preModifyFactoryNodeFunc?.Invoke(factoryNode, wasCreated, declaration) ?? factoryNode;
        var variables = wasCreated ? modifiedFactoryNode.RootFactoryMethod.Variables.Add(declaration) : modifiedFactoryNode.RootFactoryMethod.Variables;
        return (modifiedFactoryNode with { RootFactoryMethod = modifiedFactoryNode.RootFactoryMethod with { Variables = variables } }, wasCreated, declaration);
    }

    public static FactoryNode AddConstructorStatement(in this FactoryNode factoryNode, Statement statement)
    {
        return factoryNode with { FactoryImplementation = factoryNode.FactoryImplementation with { Constructor = factoryNode.FactoryImplementation.Constructor with { Statements = factoryNode.FactoryImplementation.Constructor.Statements.Add(statement) } } };
    }

    public static FactoryNode AddCreateMethodStatement(in this FactoryNode factoryNode, Statement statement)
    {
        return factoryNode with { RootFactoryMethod = factoryNode.RootFactoryMethod with { Statements = factoryNode.RootFactoryMethod.Statements.Add(statement) } };
    }
}