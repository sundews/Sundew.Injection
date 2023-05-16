// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System;
using System.Collections.Immutable;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using Parameter = Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Parameter;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

internal static class ParameterHelper
{
    public static (ImmutableList<ParameterDeclaration> Parameters, bool WasAdded, Parameter Parameter, Expression Argument) VisitParameter(
        IParameterNode parameterNode,
        string? expectedParameterName,
        ImmutableList<ParameterDeclaration> parameters,
        ImmutableList<ParameterDeclaration> additionalParameters,
        bool isMember,
        bool isOptional,
        CompilationData compilationData)
    {
        var parameterType = parameterNode.Type;
        return parameterNode.ParameterSource switch
        {
            DirectParameter direct => HandleDirect(direct, parameterNode, expectedParameterName, parameters, additionalParameters, isMember, isOptional, compilationData),
            PropertyAccessorParameter property => HandleProperty(property, parameterNode, parameterType, parameters, additionalParameters, isMember),
        };
    }

    private static (string ParameterName, bool MustNameMatchForEquality) GetParameterName(string name, IInjectionNode? parentCreationNode, Inject inject)
    {
        switch (inject)
        {
            case Inject.ByType:
                return (name.Uncapitalize(), false);
            case Inject.ByTypeAndName:
                return (name.Uncapitalize(), true);
            case Inject.Separately:
                return (NameHelper.GetUniqueName(name, parentCreationNode), true); // check for conflict
            default:
                throw new ArgumentOutOfRangeException(nameof(inject), inject, $"Case not handled: {inject}");
        }
    }

    private static (ImmutableList<ParameterDeclaration> Parameters, bool WasAdded, Parameter Parameter, Expression Argument) HandleDirect(
            DirectParameter directParameter,
            IParameterNode parameterNode,
            string? expectedParameterName,
            ImmutableList<ParameterDeclaration> parameters,
            ImmutableList<ParameterDeclaration> additionalParameters,
            bool isMember,
            bool isOptional,
            CompilationData compilationData)
    {
        var parameterType = parameterNode.RequiresNewInstance ? compilationData.FuncType.ToDefiniteBoundGenericType(ImmutableArray.Create(new DefiniteTypeArgument(parameterNode.Type, parameterNode.TypeMetadata))) : parameterNode.Type;
        var (parameterName, mustNameMatchForEquality) = GetParameterName(expectedParameterName.IsNullOrEmpty() ? parameterNode.Name : expectedParameterName, parameterNode.ParentInjectionNode, directParameter.Inject);
        var parameterDeclaration = additionalParameters.Find(x =>
            x.Type.Equals(parameterType) && x.Name == parameterName);
        var wasAdded = false;
        if (parameterDeclaration == default)
        {
            var (declarations, wasDeclarationAdded, declaration) = parameters.GetOrAddUnique(
                parameterName,
                parameterType,
                (s, i) => s + i,
                name => new ParameterDeclaration(parameterType, name, isOptional ? "null" : null));
            parameters = declarations;
            parameterDeclaration = declaration;
            wasAdded = wasDeclarationAdded;
        }
        else
        {
            isMember = true;
        }

        var parameter = new Parameter(parameterType, parameterDeclaration.Name, mustNameMatchForEquality);
        Expression argument = isMember ? new MemberAccessExpression(new Identifier("this"), parameter.Name) : new Identifier(parameter.Name);
        if (parameterNode.RequiresNewInstance)
        {
            argument = new FuncInvocationExpression(argument, isOptional);
        }

        return (parameters, wasAdded, parameter, argument);
    }

    private static (ImmutableList<ParameterDeclaration> Parameters, bool WasAdded, Parameter Parameter, Expression Argument) HandleProperty(
        PropertyAccessorParameter propertyAccessorParameter,
        IParameterNode parameterNode,
        Type parameterType,
        ImmutableList<ParameterDeclaration> parameters,
        ImmutableList<ParameterDeclaration> additionalParameters,
        bool isMember)
    {
        var variableName = NameHelper.GetVariableNameForType(propertyAccessorParameter.AccessorProperty.ContainingType);
        var parameterDeclaration = additionalParameters.Find(x =>
            x.Type.Equals(propertyAccessorParameter.AccessorProperty.ContainingType) && x.Name == variableName);
        var wasAdded = false;
        if (parameterDeclaration == default)
        {
            var (declarations, wasDeclarationAdded, declaration) = parameters.GetOrAdd(
                variableName,
                propertyAccessorParameter.AccessorProperty.ContainingType,
                (name, type) => new ParameterDeclaration(type, name, null));
            parameters = declarations;
            parameterDeclaration = declaration;
            wasAdded = wasDeclarationAdded;
        }
        else
        {
            isMember = true;
        }

        var accessorName = propertyAccessorParameter.AccessorProperty.Name;
        var parameter = new Parameter(parameterDeclaration.Type, parameterDeclaration.Name, true);
        Expression argument = isMember
            ? new MemberAccessExpression(new MemberAccessExpression(new Identifier("this"), variableName), accessorName) : new MemberAccessExpression(new Identifier(variableName), accessorName);
        if (parameterNode.RequiresNewInstance)
        {
            argument = new FuncInvocationExpression(argument, true);
        }

        return (parameters, wasAdded, parameter, argument);
    }
}