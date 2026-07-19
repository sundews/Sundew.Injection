// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage;

using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Expression = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Expression;

internal static class ParameterHelper
{
    public static (Expression Argument, bool CanAssignToField) VisitParameter(
        IParameterNode parameterNode,
        CompilationData compilationData)
    {
        return parameterNode.ParameterSource switch
        {
            DirectParameter direct => HandleDirect(direct, parameterNode, compilationData),
            PropertyAccessorParameter property => HandleProperty(property, parameterNode),
        };
    }

    private static (Expression Argument, bool CanAssignToField) HandleDirect(
            DirectParameter directParameter,
            IParameterNode parameterNode,
            CompilationData compilationData)
    {
        var isArgumentReferencedByField = parameterNode.ParameterSource.IsMember;
        Expression argument = !parameterNode.IsOptional && isArgumentReferencedByField ? new MemberAccessExpression(Identifier.This, directParameter.Name) : new Identifier(directParameter.Name);
        if (directParameter.NeedsInvocation)
        {
            argument = new FuncInvocationExpression(argument, parameterNode.IsOptional);
        }

        if (directParameter.ParameterNecessity.IsOptional && directParameter.Type.IsValueType)
        {
            argument = Expression.InvocationExpression(Expression.MemberAccessExpression(argument, "GetValueOrDefault"), []);
        }

        return (argument, !parameterNode.IsOptional);
    }

    private static (Expression Argument, bool CanAssignToField) HandleProperty(
        PropertyAccessorParameter propertyAccessorParameter,
        IParameterNode parameterNode)
    {
        var variableName = NameHelper.GetIdentifierNameForType(propertyAccessorParameter.AccessorProperty.ContainingType);
        var accessorName = propertyAccessorParameter.AccessorProperty.Name;
        Expression argument = parameterNode.ParameterSource.IsMember
            ? new MemberAccessExpression(new MemberAccessExpression(Identifier.This, variableName), accessorName) : new MemberAccessExpression(new Identifier(variableName), accessorName);
        if (propertyAccessorParameter.NeedsInvocation)
        {
            argument = new FuncInvocationExpression(argument, true);
        }

        if (propertyAccessorParameter.IsOptional && propertyAccessorParameter.AccessorProperty.ResultType.IsValueType)
        {
            argument = Expression.InvocationExpression(Expression.MemberAccessExpression(argument, "GetValueOrDefault"), []);
        }

        return (argument, true);
    }
}