// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactorySyntaxGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage;

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Sundew.Base;
using Sundew.Base.Collections;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.CodeGeneration.Templates;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;
using Sundew.Injection.Generator.TypeSystem;
using FactoryDeclarations = Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model.FactoryDeclarations;
using InjectionNode = Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes.InjectionNode;
using Member = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Member;
using MethodImplementation = Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model.MethodImplementation;
using Statement = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Statement;

internal class FactorySyntaxGenerator(
    CompilationData compilationData,
    KnownSyntax knownSyntax,
    FactoryResolvedGraph factoryResolvedGraph,
    CancellationToken cancellationToken)
{
    private const string ObjectPropertyName = "Object";
    private const string Uninitialized = "Uninitialized";
    private const string Constructed = "constructed";
    private const string Result = "Result";
    private readonly GeneratorFeatures generatorFeatures = new(new GeneratorContext(factoryResolvedGraph, compilationData, knownSyntax, cancellationToken));

    public FactoryDeclarations Generate()
    {
        var factoryImplementation = new Model.FactoryImplementation();
        var interfaces = ImmutableList.Create(compilationData.ReferencedSundewInjectionCompilationData.IGeneratedFactoryType);
        var disposeMethods = ImmutableList<Member.MethodImplementation>.Empty;
        if (factoryResolvedGraph.LifecycleHandlingInjectionTree.HasValue())
        {
            var factoryNode = this.generatorFeatures.InjectionNodeExpressionGenerator.Generate(
                factoryResolvedGraph.LifecycleHandlingInjectionTree.Root,
                factoryImplementation,
                new MethodImplementation());

            factoryImplementation = factoryNode.FactoryImplementation;

            interfaces = ImmutableList.Create(compilationData.IDisposableType)
                .Add(compilationData.IAsyncDisposableType)
                .Add(compilationData.ReferencedSundewInjectionCompilationData.IGeneratedFactoryType);

            disposeMethods = ImmutableList.Create(
                new Member.MethodImplementation(
                    new MethodDeclaration(DeclaredAccessibility.Public, false, knownSyntax.DisposeName, ImmutableList<ParameterDeclaration>.Empty, new UsedType(compilationData.VoidType)), ImmutableList.Create<Statement>(new ExpressionStatement(
                        new InvocationExpression(knownSyntax.SharedLifecycleHandler.DisposeMethod)))),
                new Member.MethodImplementation(
                    new MethodDeclaration(DeclaredAccessibility.Public, false, knownSyntax.DisposeAsyncName, ImmutableList<ParameterDeclaration>.Empty, new UsedType(compilationData.ValueTaskType)), ImmutableList.Create<Statement>(new ReturnStatement(new InvocationExpression(knownSyntax.SharedLifecycleHandler.DisposeAsyncMethod)))));
        }

        (factoryImplementation, var defaultMethodDeclarations) = factoryResolvedGraph.FactoryMethodInfos.Aggregate(
            (factoryImplementation, DefaultCreateMethods: ImmutableArray<FactoryTargetDeclaration>.Empty),
            (factory, factoryMethodInfo) =>
            {
                var result = this.GenerateFactoryMethod(
                    factoryMethodInfo.InjectionTree,
                    factoryMethodInfo,
                    factory.factoryImplementation);
                return (result.FactoryImplementation, factory.DefaultCreateMethods.Add(result.DefaultCreateMethod));
            });

        InterfaceDeclaration? interfaceDeclaration = null;
        if (factoryResolvedGraph.FactoryInterfaceType != null)
        {
            interfaceDeclaration = new InterfaceDeclaration(
                factoryResolvedGraph.FactoryInterfaceType,
                interfaces,
                ImmutableArray.Create(knownSyntax.FactoryAttribute),
                factoryImplementation.Properties.Select(x => (MemberDeclaration)x.Declaration)
                    .Concat(
                        factoryImplementation.CreateMethods.Select(x => x.Declaration),
                        factoryImplementation.DisposeMethodImplementations.Select(x => x.Declaration)).ToArray());
            interfaces = ImmutableList.Create(factoryResolvedGraph.FactoryInterfaceType);
        }

        var classDeclaration =
            new ClassDeclaration(
                factoryResolvedGraph.FactoryType,
                !factoryImplementation.FactoryMethods.Any(),
                factoryImplementation.Fields.Select(x => new Member.Field(x))
                    .Concat(
                        new Member.MethodImplementation(
                            new MethodDeclaration(DeclaredAccessibility.Public, false, factoryResolvedGraph.FactoryType.Name, factoryImplementation.Constructor.Parameters.GroupBy(x => x.DefaultValue != null).OrderBy(x => x.Key).SelectMany(x => x).ToImmutableList()),
                            factoryImplementation.Constructor.Statements).ToEnumerable<Member>(),
                        factoryImplementation.Properties.Select(x => Member._PropertyImplementation(x.Declaration, x.GetPropertyImplementation)),
                        factoryImplementation.CreateMethods.Select(x =>
                            new Member.MethodImplementation(x.Declaration, x.MethodImplementation.Statements)),
                        factoryImplementation.DisposeMethodImplementations.Select(x =>
                            new Member.MethodImplementation(x.Declaration, x.Statements)),
                        disposeMethods,
                        factoryImplementation.FactoryMethods.Select(x =>
                            new Member.MethodImplementation(x.Declaration, x.MethodImplementation.Statements)),
                        factoryImplementation.PrivateCreateMethods.Select(x =>
                            new Member.MethodImplementation(x.Declaration, x.MethodImplementation.Statements)))
                    .ToArray(),
                ImmutableArray.Create(knownSyntax.FactoryAttribute),
                interfaces);
        return new FactoryDeclarations(classDeclaration, interfaceDeclaration, defaultMethodDeclarations);
    }

    private (Model.FactoryImplementation FactoryImplementation, FactoryTargetDeclaration DefaultCreateMethod) GenerateFactoryMethod(
        InjectionNode injectionNode,
        FactoryMethodData factoryMethodData,
        in FactoryImplementation factoryImplementation)
    {
        var factoryNode = this.generatorFeatures.InjectionNodeExpressionGenerator.Generate(
            injectionNode,
            in factoryImplementation,
            new MethodImplementation());
        var targetTypeParameterName = NameHelper.GetIdentifierNameForType(factoryMethodData.Target.Type);

        var fields = factoryNode.FactoryImplementation.Fields;

        var disposeMethodImplementations = factoryImplementation.DisposeMethodImplementations;
        var factoryMethodStatements = factoryNode.CreateMethod.Statements;
        var asyncCreateReturnType = compilationData.TaskType.ToClosedGenericType(ImmutableArray.Create(new FullTypeArgument(factoryMethodData.Return)));
        var properties = factoryImplementation.Properties;
        var createMethods = factoryImplementation.CreateMethods;
        var createMethodParameters = factoryNode.CreateMethod.Parameters
            .GroupBy(x => x.DefaultValue != null)
            .OrderBy(x => x.Key)
            .SelectMany(x => x)
            .ToImmutableList();
        var createMethodDeclaration = new MethodDeclaration(
            DeclaredAccessibility.Public,
            false,
            factoryMethodData.FactoryMethodName,
            createMethodParameters,
            new UsedType(factoryMethodData.Return.Type));

        var isProperty = factoryMethodData.InjectionTree is SingleInstancePerFactoryInjectionNode;

        if (factoryMethodData.RootNeedsLifecycleHandling)
        {
            if (isProperty)
            {
                var createPropertyDeclaration = new PropertyDeclaration(
                    factoryMethodData.Return.Type,
                    factoryMethodData.FactoryMethodName,
                    ImmutableList.Create(knownSyntax.BindableCreateMethodAttribute));

                factoryMethodStatements = factoryMethodStatements.Add(Statement.ExpressionStatement(
                    new InvocationExpression(knownSyntax.SharedLifecycleHandler.InitializeMethod)));
                factoryMethodStatements =
                    factoryMethodStatements.Add(
                        new ReturnStatement(factoryNode.DependantArguments.Single()));
                properties = properties
                    .Add(new DeclaredPropertyImplementation(createPropertyDeclaration, factoryMethodStatements));
            }
            else
            {
                factoryMethodStatements = factoryMethodStatements.Insert(0, knownSyntax.SharedLifecycleHandler.CreateChildLifecycleHandlerAndAssignVarStatement);

                var constructedValueVariableName = injectionNode.GetInjectionNodeName().Uncapitalize() + Result;
                var constructedValueIdentifier = new Identifier(constructedValueVariableName);
                factoryMethodStatements = factoryMethodStatements.Add(
                        new LocalDeclarationStatement(
                            constructedValueVariableName,
                            factoryNode.DependantArguments.Single()))
                    .Add(
                        new ExpressionStatement(
                            new InvocationExpression(
                                knownSyntax.SharedLifecycleHandler.TryAddMethod,
                                [constructedValueIdentifier, knownSyntax.ChildLifecycleHandler.Access,])));

                var constructedType = compilationData.ReferencedSundewInjectionCompilationData.ConstructedType.ToClosedGenericType(
                        ImmutableArray.Create(new FullTypeArgument(factoryMethodData.Return)));
                var createMethodAsyncDeclaration = new MethodDeclaration(
                    DeclaredAccessibility.Public,
                    false,
                    true,
                    factoryMethodData.FactoryMethodName + Trivia.AsyncName,
                    createMethodParameters,
                    ImmutableArray.Create(knownSyntax.IndirectCreateMethodAttribute),
                    new UsedType(asyncCreateReturnType));
                var createUnInitializedMethodDeclaration = new MethodDeclaration(
                    DeclaredAccessibility.Public,
                    false,
                    false,
                    factoryMethodData.FactoryMethodName + Uninitialized,
                    createMethodParameters,
                    ImmutableArray.Create(knownSyntax.EditorBrowsableAttribute, knownSyntax.BindableCreateMethodAttribute, knownSyntax.IndirectCreateMethodAttribute),
                    new UsedType(constructedType));
                var createStatement =
                    new LocalDeclarationStatement(
                        Constructed + factoryMethodData.Target.Type.Name,
                        new InvocationExpression(
                            new MemberAccessExpression(Identifier.This, createUnInitializedMethodDeclaration.Name),
                            createMethodParameters.Select(x => new Identifier(x.Name))
                                .ToImmutableArray()));
                createMethods = createMethods
                    .Add(new DeclaredMethodImplementation(createMethodDeclaration, factoryNode.CreateMethod with
                    {
                        Statements = ImmutableList.Create<Statement>(createStatement)
                            .Add(Statement.ExpressionStatement(
                                new InvocationExpression(knownSyntax.SharedLifecycleHandler.InitializeMethod)))
                            .Add(new ReturnStatement(new MemberAccessExpression(new Identifier(createStatement.Name), ObjectPropertyName))),
                    }))
                    .Add(new DeclaredMethodImplementation(createMethodAsyncDeclaration, factoryNode.CreateMethod with
                    {
                        Statements = ImmutableList.Create<Statement>(createStatement)
                            .Add(Statement.ExpressionStatement(knownSyntax.SharedLifecycleHandler
                                .InitializeAsyncMethodCall))
                            .Add(new ReturnStatement(new MemberAccessExpression(new Identifier(createStatement.Name), ObjectPropertyName))),
                    }));
                createMethods = createMethods.Add(new DeclaredMethodImplementation(
                    createUnInitializedMethodDeclaration,
                    factoryNode.CreateMethod with
                    {
                        Statements = factoryMethodStatements.Add(new ReturnStatement(
                            CreationExpression._ConstructorCall(constructedType, ImmutableArray.Create(new Identifier(constructedValueVariableName), knownSyntax.ChildLifecycleHandler.Access)))),
                    }));

                disposeMethodImplementations = disposeMethodImplementations.Add(
                    new DeclaredDisposeMethodImplementation(
                        new MethodDeclaration(
                            DeclaredAccessibility.Public,
                            false,
                            knownSyntax.DisposeName,
                            ImmutableList.Create(new ParameterDeclaration(factoryMethodData.Return.Type, targetTypeParameterName)),
                            new UsedType(compilationData.VoidType)),
                        ImmutableList.Create<Statement>(new ExpressionStatement(
                            new InvocationExpression(
                                knownSyntax.SharedLifecycleHandler.DisposeMethod,
                                [new Identifier(targetTypeParameterName)])))));

                disposeMethodImplementations = disposeMethodImplementations.Add(
                    new DeclaredDisposeMethodImplementation(
                        new MethodDeclaration(
                            DeclaredAccessibility.Public,
                            false,
                            knownSyntax.DisposeAsyncName,
                            ImmutableList.Create(new ParameterDeclaration(factoryMethodData.Return.Type, targetTypeParameterName)),
                            new UsedType(compilationData.ValueTaskType)),
                        ImmutableList.Create<Statement>(new ReturnStatement(
                            new InvocationExpression(
                                knownSyntax.SharedLifecycleHandler.DisposeAsyncMethod,
                                [new Identifier(targetTypeParameterName)])))));
            }
        }
        else
        {
            if (isProperty)
            {
                var createPropertyDeclaration = new PropertyDeclaration(
                    factoryMethodData.Return.Type,
                    factoryMethodData.FactoryMethodName,
                    ImmutableList.Create(knownSyntax.BindableCreateMethodAttribute));
                factoryMethodStatements =
                    factoryMethodStatements.Add(
                        new ReturnStatement(factoryNode.DependantArguments.Single()));
                properties = properties
                    .Add(new DeclaredPropertyImplementation(createPropertyDeclaration, factoryMethodStatements));
            }
            else
            {
                factoryMethodStatements =
                    factoryMethodStatements.Add(
                        new ReturnStatement(factoryNode.DependantArguments.Single()));
                createMethods = createMethods
                    .Add(new DeclaredMethodImplementation(
                        createMethodDeclaration with { Attributes = ImmutableList.Create(knownSyntax.BindableCreateMethodAttribute), }, factoryNode.CreateMethod with { Statements = factoryMethodStatements }));
            }
        }

        return (factoryNode.FactoryImplementation with
        {
            Fields = fields,
            Properties = properties,
            CreateMethods = createMethods,
            DisposeMethodImplementations = disposeMethodImplementations,
        },
            new FactoryTargetDeclaration(createMethodDeclaration.Name, createMethodDeclaration.Parameters, factoryMethodData.Return.Type, isProperty));
    }
}