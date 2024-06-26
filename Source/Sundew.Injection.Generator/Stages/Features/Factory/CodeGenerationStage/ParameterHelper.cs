﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage;

using System;
using System.Collections.Immutable;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using Expression = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Expression;
using Parameter = Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model.Parameter;

internal static class ParameterHelper
{
    private const string Null = "null";

    public static (ImmutableList<ParameterDeclaration> Parameters, bool WasAdded, Parameter Parameter, Expression Argument, bool CanAssignToField) VisitParameter(
        IParameterNode parameterNode,
        string? expectedParameterName,
        ImmutableList<ParameterDeclaration> parameters,
        ImmutableList<ParameterDeclaration> additionalParameters,
        CompilationData compilationData)
    {
        return parameterNode.ParameterSource switch
        {
            DirectParameter direct => HandleDirect(direct, parameterNode, expectedParameterName, parameters, additionalParameters, compilationData),
            PropertyAccessorParameter property => HandleProperty(property, parameterNode, parameters, additionalParameters),
        };
    }

    private static (string ParameterName, bool MustNameMatchForEquality) GetParameterName(string name, string? parentName, Inject inject)
    {
        return inject switch
        {
            Inject.Shared => (name.Uncapitalize(), false),
            Inject.ByParameterName => (name.Uncapitalize(), true),
            Inject.Separately => (NameHelper.GetDependantScopedName(name, parentName), true), // check for conflict
            _ => throw new ArgumentOutOfRangeException(nameof(inject), inject, $"Case not handled: {inject}"),
        };
    }

    private static (ImmutableList<ParameterDeclaration> Parameters, bool WasAdded, Parameter Parameter, Expression Argument, bool CanAssignToField) HandleDirect(
            DirectParameter directParameter,
            IParameterNode parameterNode,
            string? expectedParameterName,
            ImmutableList<ParameterDeclaration> parameters,
            ImmutableList<ParameterDeclaration> additionalParameters,
            CompilationData compilationData)
    {
        var parameterType = parameterNode.PrefersNewInstance ? compilationData.FuncType.ToClosedGenericType(ImmutableArray.Create(new FullTypeArgument(parameterNode.Type, parameterNode.TypeMetadata))) : parameterNode.Type;
        var (parameterName, mustNameMatchForEquality) = GetParameterName(expectedParameterName.IsNullOrEmpty() ? parameterNode.Name : expectedParameterName, parameterNode.DependantName, directParameter.Inject);
        var parameterDeclaration = additionalParameters.Find(x =>
            x.Type.Equals(parameterType) && x.Name == parameterName);
        var wasAdded = false;
        var isArgumentReferencedByField = parameterNode.IsForConstructor;
        if (parameterDeclaration == default)
        {
            var (declarations, wasDeclarationAdded, declaration) = parameters.GetOrAdd(
                parameterName,
                parameterType,
                name => new ParameterDeclaration(parameterType, name, parameterNode.IsOptional ? Null : null));
            parameters = declarations;
            parameterDeclaration = declaration;
            wasAdded = wasDeclarationAdded;
        }
        else
        {
            isArgumentReferencedByField = true;
        }

        var parameter = new Parameter(parameterType, parameterDeclaration.Name, mustNameMatchForEquality);
        Expression argument = !parameterNode.IsOptional && isArgumentReferencedByField ? new MemberAccessExpression(Identifier.This, parameter.Name) : new Identifier(parameter.Name);
        if (parameterNode.PrefersNewInstance)
        {
            argument = new FuncInvocationExpression(argument, parameterNode.IsOptional);
        }

        return (parameters, wasAdded, parameter, argument, !parameterNode.IsOptional);
    }

    private static (ImmutableList<ParameterDeclaration> Parameters, bool WasAdded, Parameter Parameter, Expression Argument, bool CanAssignToField) HandleProperty(
        PropertyAccessorParameter propertyAccessorParameter,
        IParameterNode parameterNode,
        ImmutableList<ParameterDeclaration> parameters,
        ImmutableList<ParameterDeclaration> additionalParameters)
    {
        var variableName = NameHelper.GetIdentifierNameForType(propertyAccessorParameter.AccessorProperty.ContainingType);
        var parameterDeclaration = additionalParameters.Find(x =>
            x.Type.Equals(propertyAccessorParameter.AccessorProperty.ContainingType) && x.Name == variableName);
        var wasAdded = false;
        var isMember = parameterNode.IsForConstructor;
        if (parameterDeclaration == default)
        {
            (parameters, wasAdded, parameterDeclaration) = parameters.GetOrAdd(
                variableName,
                propertyAccessorParameter.AccessorProperty.ContainingType,
                (name, type) => new ParameterDeclaration(type, name, null));
        }
        else
        {
            isMember = true;
        }

        var accessorName = propertyAccessorParameter.AccessorProperty.Name;
        var parameter = new Parameter(parameterDeclaration.Type, parameterDeclaration.Name, true);
        Expression argument = isMember
            ? new MemberAccessExpression(new MemberAccessExpression(Identifier.This, variableName), accessorName) : new MemberAccessExpression(new Identifier(variableName), accessorName);
        if (propertyAccessorParameter.NeedsInvocation)
        {
            argument = new FuncInvocationExpression(argument, true);
        }

        return (parameters, wasAdded, parameter, argument, true);
    }
}