// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionNodeExpressionGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage;

using System.Threading;
using Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;
using InjectionNode = Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes.InjectionNode;

internal class InjectionNodeEvaluator
{
    private readonly CancellationToken cancellationToken;
    private readonly NewInstanceGenerator newInstanceGenerator;
    private readonly SingleInstancePerRequestGenerator singleInstancePerRequestGenerator;
    private readonly SingleInstancePerFactoryGenerator singleInstancePerFactoryGenerator;
    private readonly SingleInstancePerObjectGenerator singleInstancePerObjectGenerator;
    private readonly FactoryMethodParameterGenerator factoryMethodParameterGenerator;
    private readonly FactoryConstructorParameterGenerator factoryConstructorParameterGenerator;

    public InjectionNodeEvaluator(GeneratorFeatures generatorFeatures, GeneratorContext generatorContext)
    {
        this.newInstanceGenerator = new NewInstanceGenerator(generatorFeatures, generatorContext);
        this.singleInstancePerRequestGenerator = new SingleInstancePerRequestGenerator(generatorFeatures, generatorContext);
        this.singleInstancePerFactoryGenerator = new SingleInstancePerFactoryGenerator(generatorFeatures, generatorContext);
        this.singleInstancePerObjectGenerator = new SingleInstancePerObjectGenerator(generatorFeatures, generatorContext);
        this.factoryMethodParameterGenerator = new FactoryMethodParameterGenerator(generatorFeatures, generatorContext);
        this.factoryConstructorParameterGenerator = new FactoryConstructorParameterGenerator(generatorFeatures, generatorContext);
    }

    public FactoryNode Generate(
        InjectionNode injectionNode,
        in FactoryImplementation factoryImplementation,
        in MethodImplementation methodImplementation)
    {
        this.cancellationToken.ThrowIfCancellationRequested();
        return injectionNode switch
        {
            SingleInstancePerObjectInjectionNode singleInstancePerThreadCreationNode => this.singleInstancePerObjectGenerator.VisitSingleInstancePerObject(
                singleInstancePerThreadCreationNode,
                in factoryImplementation,
                in methodImplementation),
            SingleInstancePerRequestInjectionNode singleInstancePerRequestCreationNode => this.singleInstancePerRequestGenerator.VisitSingleInstancePerRequest(
                singleInstancePerRequestCreationNode,
                in factoryImplementation,
                in methodImplementation),
            SingleInstancePerFactoryInjectionNode singleInstancePerFactoryCreationNode => this.singleInstancePerFactoryGenerator.VisitSingleInstancePerFactory(
                singleInstancePerFactoryCreationNode,
                in factoryImplementation,
                in methodImplementation),
            NewInstanceInjectionNode newInstanceCreationNode => this.newInstanceGenerator.VisitNewInstance(
                newInstanceCreationNode,
                in factoryImplementation,
                in methodImplementation),
            FactoryConstructorParameterInjectionNode constructorParameterCreationNode => this.factoryConstructorParameterGenerator.VisitFactoryConstructorParameter(
                constructorParameterCreationNode,
                in factoryImplementation,
                in methodImplementation),
            FactoryMethodParameterInjectionNode factoryMethodParameterCreationNode => this.factoryMethodParameterGenerator.VisitFactoryMethodParameter(
                factoryMethodParameterCreationNode,
                in factoryImplementation,
                in methodImplementation),
        };
    }
}