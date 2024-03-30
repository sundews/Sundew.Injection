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
using Expression = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Expression;
using FactoryDeclarations = Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model.FactoryDeclarations;
using InjectionNode = Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes.InjectionNode;
using Member = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Member;
using MethodImplementation = Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model.MethodImplementation;
using Statement = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Statement;

internal class FactorySyntaxGenerator
{
    private const string ObjectPropertyName = "Object";
    private const string Uninitialized = "Uninitialized";
    private const string Constructed = "constructed";
    private const string Result = "Result";
    private readonly CompilationData compilationData;
    private readonly KnownSyntax knownSyntax;
    private readonly FactoryResolvedGraph factoryResolvedGraph;
    private readonly GeneratorFeatures generatorFeatures;

    public FactorySyntaxGenerator(
        CompilationData compilationData,
        KnownSyntax knownSyntax,
        FactoryResolvedGraph factoryResolvedGraph,
        CancellationToken cancellationToken)
    {
        this.compilationData = compilationData;
        this.knownSyntax = knownSyntax;
        this.factoryResolvedGraph = factoryResolvedGraph;
        this.generatorFeatures = new GeneratorFeatures(new GeneratorContext(factoryResolvedGraph, compilationData, knownSyntax, cancellationToken));
    }

    public FactoryDeclarations Generate()
    {
        var factoryImplementation = new Model.FactoryImplementation();
        var interfaces = ImmutableList.Create(this.compilationData.IGeneratedFactoryType);
        var disposeMethods = ImmutableList<Member.MethodImplementation>.Empty;
        if (this.factoryResolvedGraph.LifecycleHandlingInjectionTree.HasValue())
        {
            var factoryNode = this.generatorFeatures.InjectionNodeExpressionGenerator.Generate(
                this.factoryResolvedGraph.LifecycleHandlingInjectionTree.Root,
                factoryImplementation,
                new MethodImplementation());

            factoryImplementation = factoryNode.FactoryImplementation;

            interfaces = ImmutableList.Create(this.compilationData.IDisposableType)
                .Add(this.compilationData.IAsyncDisposableType)
                .Add(this.compilationData.IGeneratedFactoryType);

            disposeMethods = ImmutableList.Create(
                new Member.MethodImplementation(
                    new MethodDeclaration(DeclaredAccessibility.Public, false, this.knownSyntax.DisposeName, ImmutableList<ParameterDeclaration>.Empty, new UsedType(this.compilationData.VoidType)), ImmutableList.Create<Statement>(new ExpressionStatement(
                        new InvocationExpression(this.knownSyntax.SharedLifecycleHandler.DisposeMethod)))),
                new Member.MethodImplementation(
                    new MethodDeclaration(DeclaredAccessibility.Public, false, this.knownSyntax.DisposeAsyncName, ImmutableList<ParameterDeclaration>.Empty, new UsedType(this.compilationData.ValueTaskType)), ImmutableList.Create<Statement>(new ReturnStatement(new InvocationExpression(this.knownSyntax.SharedLifecycleHandler.DisposeAsyncMethod)))));
        }

        (factoryImplementation, var defaultMethodDeclarations) = this.factoryResolvedGraph.FactoryMethodInfos.Aggregate(
            (factoryImplementation, DefaultCreateMethods: ImmutableArray<DefiniteFactoryMethod>.Empty),
            (factory, factoryMethodInfo) =>
            {
                var result = this.GenerateFactoryMethod(
                    factoryMethodInfo.InjectionTree,
                    factoryMethodInfo,
                    factory.factoryImplementation);
                return (result.FactoryImplementation, factory.DefaultCreateMethods.Add(result.DefaultCreateMethod));
            });

        InterfaceDeclaration? interfaceDeclaration = null;
        if (this.factoryResolvedGraph.FactoryInterfaceType != null)
        {
            interfaceDeclaration = new InterfaceDeclaration(
                this.factoryResolvedGraph.FactoryInterfaceType,
                interfaces,
                ImmutableArray.Create(this.knownSyntax.FactoryAttribute),
                factoryImplementation.CreateMethods.Select(x => x.Declaration)
                    .Concat(
                        factoryImplementation.DisposeMethodImplementations.Select(x => x.Declaration)).ToArray());
            interfaces = ImmutableList.Create(this.factoryResolvedGraph.FactoryInterfaceType);
        }

        var classDeclaration =
            new ClassDeclaration(
                this.factoryResolvedGraph.FactoryType,
                !factoryImplementation.FactoryMethods.Any(),
                factoryImplementation.Fields.Select(x => new Member.Field(x))
                    .Concat(
                        new Member.MethodImplementation(
                            new MethodDeclaration(DeclaredAccessibility.Public, false, this.factoryResolvedGraph.FactoryType.Name, factoryImplementation.Constructor.Parameters.GroupBy(x => x.DefaultValue != null).OrderBy(x => x.Key).SelectMany(x => x).ToImmutableList()),
                            factoryImplementation.Constructor.Statements).ToEnumerable<Member>(),
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
                ImmutableArray.Create(this.knownSyntax.FactoryAttribute),
                interfaces);
        return new FactoryDeclarations(classDeclaration, interfaceDeclaration, defaultMethodDeclarations);
    }

    private (Model.FactoryImplementation FactoryImplementation, DefiniteFactoryMethod DefaultCreateMethod) GenerateFactoryMethod(
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
        var asyncCreateReturnType = this.compilationData.TaskType.ToDefiniteClosedGenericType(ImmutableArray.Create(new DefiniteTypeArgument(factoryMethodData.Return)));
        var factoryMethods = factoryImplementation.CreateMethods;
        var createMethodParameters = factoryNode.CreateMethod.Parameters.GroupBy(x => x.DefaultValue != null).OrderBy(x => x.Key).SelectMany(x => x).ToImmutableList();
        var createMethodDeclaration = new MethodDeclaration(
            DeclaredAccessibility.Public,
            false,
            factoryMethodData.FactoryMethodName,
            createMethodParameters,
            new UsedType(factoryMethodData.Return.Type));

        if (factoryMethodData.RootNeedsLifecycleHandling)
        {
            factoryMethodStatements = factoryMethodStatements.Insert(0, this.knownSyntax.SharedLifecycleHandler.CreateChildLifecycleHandlerAndAssignVarStatement);

            var constructedValueVariableName = injectionNode.GetInjectionNodeName().Uncapitalize() + Result;
            var constructedValueIdentifier = new Identifier(constructedValueVariableName);
            factoryMethodStatements = factoryMethodStatements.Add(
                    new LocalDeclarationStatement(
                        constructedValueVariableName,
                        factoryNode.DependeeArguments.Single()))
                .Add(
                    new ExpressionStatement(
                        new InvocationExpression(
                            this.knownSyntax.SharedLifecycleHandler.TryAddMethod,
                            new[] { constructedValueIdentifier, this.knownSyntax.ChildLifecycleHandler.Access, })));

            var constructedType =
                this.compilationData.ConstructedType.ToDefiniteClosedGenericType(
                    ImmutableArray.Create(new DefiniteTypeArgument(factoryMethodData.Return)));
            var createMethodAsyncDeclaration = new MethodDeclaration(
                DeclaredAccessibility.Public,
                false,
                true,
                factoryMethodData.FactoryMethodName + Trivia.AsyncName,
                createMethodParameters,
                ImmutableArray.Create(this.knownSyntax.IndirectCreateMethodAttribute),
                new UsedType(asyncCreateReturnType));
            var createUnInitializedMethodDeclaration = new MethodDeclaration(
                DeclaredAccessibility.Public,
                false,
                false,
                factoryMethodData.FactoryMethodName + Uninitialized,
                createMethodParameters,
                ImmutableArray.Create(this.knownSyntax.EditorBrowsableAttribute, this.knownSyntax.BindableCreateMethodAttribute, this.knownSyntax.IndirectCreateMethodAttribute),
                new UsedType(constructedType));
            var createStatement =
                new LocalDeclarationStatement(
                    Constructed + factoryMethodData.Target.Type.Name,
                    new InvocationExpression(
                        new MemberAccessExpression(Identifier.This, createUnInitializedMethodDeclaration.Name),
                        createMethodParameters.Select(x => new Identifier(x.Name))
                            .ToImmutableArray()));
            factoryMethods = factoryMethods
                .Add(new DeclaredMethodImplementation(createMethodDeclaration, factoryNode.CreateMethod with
                {
                    Statements = ImmutableList.Create<Statement>(createStatement)
                        .Add(Statement.ExpressionStatement(new InvocationExpression(this.knownSyntax.SharedLifecycleHandler.InitializeMethod)))
                        .Add(new ReturnStatement(new MemberAccessExpression(new Identifier(createStatement.Name), ObjectPropertyName))),
                }))
                .Add(new DeclaredMethodImplementation(createMethodAsyncDeclaration, factoryNode.CreateMethod with
                {
                    Statements = ImmutableList.Create<Statement>(createStatement)
                        .Add(Statement.ExpressionStatement(this.knownSyntax.SharedLifecycleHandler.InitializeAsyncMethodCall))
                        .Add(new ReturnStatement(new MemberAccessExpression(new Identifier(createStatement.Name), ObjectPropertyName))),
                }));
            factoryMethods = factoryMethods.Add(new DeclaredMethodImplementation(
                createUnInitializedMethodDeclaration,
                factoryNode.CreateMethod with
                {
                    Statements = factoryMethodStatements.Add(new ReturnStatement(CreationExpression._ConstructorCall(constructedType, ImmutableArray.Create(new Identifier(constructedValueVariableName), this.knownSyntax.ChildLifecycleHandler.Access)))),
                }));

            disposeMethodImplementations = disposeMethodImplementations.Add(
                new DeclaredDisposeMethodImplementation(
                    new MethodDeclaration(
                        DeclaredAccessibility.Public,
                        false,
                        this.knownSyntax.DisposeName,
                        ImmutableList.Create(new ParameterDeclaration(factoryMethodData.Return.Type, targetTypeParameterName)),
                        new UsedType(this.compilationData.VoidType)),
                    ImmutableList.Create<Statement>(new ExpressionStatement(
                        new InvocationExpression(
                            this.knownSyntax.SharedLifecycleHandler.DisposeMethod,
                            new Expression[] { new Identifier(targetTypeParameterName) })))));

            disposeMethodImplementations = disposeMethodImplementations.Add(
                new DeclaredDisposeMethodImplementation(
                    new MethodDeclaration(DeclaredAccessibility.Public, false, this.knownSyntax.DisposeAsyncName, ImmutableList.Create(new ParameterDeclaration(factoryMethodData.Return.Type, targetTypeParameterName)), new UsedType(this.compilationData.ValueTaskType)),
                    ImmutableList.Create<Statement>(new ReturnStatement(
                        new InvocationExpression(
                            this.knownSyntax.SharedLifecycleHandler.DisposeAsyncMethod,
                            new Expression[] { new Identifier(targetTypeParameterName) })))));
        }
        else
        {
            factoryMethodStatements =
                factoryMethodStatements.Add(
                    new ReturnStatement(factoryNode.DependeeArguments.Single()));
            factoryMethods = factoryMethods
                .Add(new DeclaredMethodImplementation(createMethodDeclaration with { Attributes = ImmutableList.Create(this.knownSyntax.BindableCreateMethodAttribute) }, factoryNode.CreateMethod with { Statements = factoryMethodStatements }));
        }

        return (factoryNode.FactoryImplementation with
        {
            Fields = fields,
            CreateMethods = factoryMethods,
            DisposeMethodImplementations = disposeMethodImplementations,
        },
            new DefiniteFactoryMethod(createMethodDeclaration.Name, createMethodDeclaration.Parameters, factoryMethodData.Return.Type));
    }
}