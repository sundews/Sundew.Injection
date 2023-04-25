// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethodParameterGenerator.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

internal class FactoryMethodParameterGenerator
{
    private readonly InjectionNodeEvaluator injectionNodeEvaluator;
    private readonly CompilationData compilationData;
    private readonly KnownSyntax knownSyntax;

    public FactoryMethodParameterGenerator(InjectionNodeEvaluator injectionNodeEvaluator, CompilationData compilationData, KnownSyntax knownSyntax)
    {
        this.injectionNodeEvaluator = injectionNodeEvaluator;
        this.compilationData = compilationData;
        this.knownSyntax = knownSyntax;
    }

    public FactoryNode VisitFactoryMethodParameter(
        FactoryMethodParameterInjectionNode factoryMethodParameterInjectionNode,
        FactoryData factoryModel,
        in FactoryImplementation factoryImplementation,
        in MethodImplementation method)
    {
        var (parameters, _, _, argument) = ParameterHelper.VisitParameter(
            factoryMethodParameterInjectionNode,
            null,
            method.Parameters,
            factoryImplementation.Constructor.Parameters,
            false,
            false,
            this.compilationData);
        return new FactoryNode(
            factoryImplementation,
            method with
            {
                Parameters = parameters,
            },
            ImmutableList.Create(argument));
    }
}