// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryNodeExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;

using System;
using System.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;
using Sundew.Injection.Generator.TypeSystem;
using Expression = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Expression;
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
        var (wasCreated, declaration) = factoryNode.CreateMethod.Variables.GetOrCreate(
            name,
            type,
            createDeclarationFunc);
        var modifiedFactoryNode = preModifyFactoryNodeFunc?.Invoke(factoryNode, wasCreated, declaration) ?? factoryNode;
        var variables = wasCreated ? modifiedFactoryNode.CreateMethod.Variables.Add(declaration) : modifiedFactoryNode.CreateMethod.Variables;
        return (modifiedFactoryNode with { CreateMethod = modifiedFactoryNode.CreateMethod with { Variables = variables } }, wasCreated, declaration);
    }

    public static (FactoryNode FactoryNode, bool WasAdded, Parameter Parameter, Expression Argument, bool CanAssignToField) GetOrAddConstructorParameter(
        in this FactoryNode factoryNode,
        IParameterNode parameterNode,
        string? expectedParameterName,
        ImmutableList<ParameterDeclaration> additionalParameters,
        CompilationData compilationData)
    {
        var (parameters, wasAdded, parameter, argument, canAssignToField) = ParameterHelper.VisitParameter(
            parameterNode,
            expectedParameterName,
            factoryNode.FactoryImplementation.Constructor.Parameters,
            additionalParameters,
            compilationData);

        return (factoryNode with { FactoryImplementation = factoryNode.FactoryImplementation with { Constructor = factoryNode.FactoryImplementation.Constructor with { Parameters = parameters } } }, wasAdded, parameter, argument, canAssignToField);
    }

    public static (FactoryNode FactoryNode, bool WasAdded, Parameter Parameter, Expression Argument, bool CanAssignToField) GetOrAddCreateMethodParameter(
        in this FactoryNode factoryNode,
        IParameterNode parameterNode,
        string? expectedParameterName,
        CompilationData compilationData)
    {
        var (parameters, wasAdded, parameter, argument, canAssignToField) = ParameterHelper.VisitParameter(
            parameterNode,
            expectedParameterName,
            factoryNode.CreateMethod.Parameters,
            factoryNode.FactoryImplementation.Constructor.Parameters,
            compilationData);

        return (factoryNode with { CreateMethod = factoryNode.CreateMethod with { Parameters = parameters } }, wasAdded, parameter, argument, canAssignToField);
    }

    public static FactoryNode AddConstructorStatement(in this FactoryNode factoryNode, Statement statement)
    {
        return factoryNode with { FactoryImplementation = factoryNode.FactoryImplementation with { Constructor = factoryNode.FactoryImplementation.Constructor with { Statements = factoryNode.FactoryImplementation.Constructor.Statements.Add(statement) } } };
    }

    public static FactoryNode AddCreateMethodStatement(in this FactoryNode factoryNode, Statement statement)
    {
        return factoryNode with { CreateMethod = factoryNode.CreateMethod with { Statements = factoryNode.CreateMethod.Statements.Add(statement) } };
    }
}