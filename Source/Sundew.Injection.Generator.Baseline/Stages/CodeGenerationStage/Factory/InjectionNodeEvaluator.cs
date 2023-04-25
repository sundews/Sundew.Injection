// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionNodeEvaluator.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System.Threading;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;
using InjectionNode = Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes.InjectionNode;

internal class InjectionNodeEvaluator
{
    private readonly CancellationToken cancellationToken;
    private readonly NewInstanceGenerator newInstanceGenerator;
    private readonly SingleInstancePerRequestGenerator singleInstancePerRequestGenerator;
    private readonly SingleInstancePerFactoryGenerator singleInstancePerFactoryGenerator;
    private readonly SingleInstancePerObjectGenerator singleInstancePerObjectGenerator;
    private readonly FactoryMethodParameterGenerator factoryMethodParameterGenerator;
    private readonly FactoryConstructorParameterGenerator factoryConstructorParameterGenerator;

    public InjectionNodeEvaluator(
        KnownSyntax knownSyntax,
        CompilationData compilationData,
        CancellationToken cancellationToken)
    {
        this.cancellationToken = cancellationToken;
        this.newInstanceGenerator = new NewInstanceGenerator(this, compilationData, knownSyntax);
        this.singleInstancePerRequestGenerator = new SingleInstancePerRequestGenerator(this, compilationData, knownSyntax);
        this.singleInstancePerFactoryGenerator = new SingleInstancePerFactoryGenerator(this, compilationData, knownSyntax);
        this.singleInstancePerObjectGenerator = new SingleInstancePerObjectGenerator(this, knownSyntax);
        this.factoryMethodParameterGenerator = new FactoryMethodParameterGenerator(this, compilationData, knownSyntax);
        this.factoryConstructorParameterGenerator = new FactoryConstructorParameterGenerator(this, compilationData, knownSyntax);
    }

    public FactoryNode Evaluate(
        InjectionNode injectionNode,
        FactoryData factoryData,
        in FactoryImplementation factoryImplementation,
        in MethodImplementation methodImplementation)
    {
        this.cancellationToken.ThrowIfCancellationRequested();
        return injectionNode switch
        {
            SingleInstancePerObjectInjectionNode singleInstancePerThreadCreationNode => this.singleInstancePerObjectGenerator.VisitSingleInstancePerObject(
                singleInstancePerThreadCreationNode,
                factoryData,
                in factoryImplementation,
                in methodImplementation),
            SingleInstancePerRequestInjectionNode singleInstancePerRequestCreationNode => this.singleInstancePerRequestGenerator.VisitSingleInstancePerRequest(
                singleInstancePerRequestCreationNode,
                factoryData,
                in factoryImplementation,
                in methodImplementation),
            SingleInstancePerFactoryInjectionNode singleInstancePerFactoryCreationNode => this.singleInstancePerFactoryGenerator.VisitSingleInstancePerFactory(
                singleInstancePerFactoryCreationNode,
                factoryData,
                in factoryImplementation,
                in methodImplementation),
            NewInstanceInjectionNode newInstanceCreationNode => this.newInstanceGenerator.VisitNewInstance(
                newInstanceCreationNode,
                factoryData,
                in factoryImplementation,
                in methodImplementation),
            FactoryConstructorParameterInjectionNode constructorParameterCreationNode => this.factoryConstructorParameterGenerator.VisitFactoryConstructorParameter(
                constructorParameterCreationNode,
                factoryData,
                in factoryImplementation,
                in methodImplementation),
            FactoryMethodParameterInjectionNode factoryMethodParameterCreationNode => this.factoryMethodParameterGenerator.VisitFactoryMethodParameter(
                factoryMethodParameterCreationNode,
                factoryData,
                in factoryImplementation,
                in methodImplementation),
        };
    }
}