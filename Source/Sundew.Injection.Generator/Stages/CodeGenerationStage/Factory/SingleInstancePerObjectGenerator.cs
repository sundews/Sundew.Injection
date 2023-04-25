// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleInstancePerObjectGenerator.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.FactoryDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

internal class SingleInstancePerObjectGenerator
{
    private readonly InjectionNodeEvaluator injectionNodeEvaluator;
    private readonly KnownSyntax knownSyntax;

    public SingleInstancePerObjectGenerator(InjectionNodeEvaluator injectionNodeEvaluator, KnownSyntax knownSyntax)
    {
        this.injectionNodeEvaluator = injectionNodeEvaluator;
        this.knownSyntax = knownSyntax;
    }

    public FactoryNode VisitSingleInstancePerObject(
        SingleInstancePerObjectInjectionNode singleInstancePerObjectInjectionNode,
        FactoryData factoryModel,
        in FactoryImplementation factoryImplementation,
        in MethodImplementation method)
    {
        throw new NotImplementedException();
    }
}