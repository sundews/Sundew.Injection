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
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.Stages.CodeGeneration.Templates;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
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
        var factoryImplementation = new FactoryImplementation(
            new Constructor(
            factoryResolvedGraph.DeclaredConstructor.Parameters.Select(x =>
                new ParameterDeclaration(x.Type, x.Name, x.ParameterNecessity)).ToImmutableList()));

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
                    new MethodDeclaration(DeclaredAccessibility.Public, false, false, false, knownSyntax.DisposeName, ImmutableList<ParameterDeclaration>.Empty, new UsedType(compilationData.VoidType)), ImmutableList.Create<Statement>(new ExpressionStatement(
                        new InvocationExpression(knownSyntax.SharedLifecycleHandler.DisposeMethod)))),
                new Member.MethodImplementation(
                    new MethodDeclaration(DeclaredAccessibility.Public, false, false, false, knownSyntax.DisposeAsyncName, ImmutableList<ParameterDeclaration>.Empty, new UsedType(compilationData.ValueTaskType)), ImmutableList.Create<Statement>(new ReturnStatement(new InvocationExpression(knownSyntax.SharedLifecycleHandler.DisposeAsyncMethod)))));
        }

        (factoryImplementation, var interfaceDeclarations, var defaultMethodDeclarations) = factoryResolvedGraph.ResolvedRootFactoryMethods.Aggregate(
            (factoryImplementation, InterfaceDeclarations: ImmutableArray<InterfaceDeclaration>.Empty, DefaultCreateMethods: ImmutableArray<FactoryTargetDeclaration>.Empty),
            (factory, pair) =>
            {
                (factoryImplementation, var defaultMethodDeclarations, var interfaceMembers) = pair.Value.Aggregate(
                    (factoryImplementation, DefaultCreateMethods: ImmutableArray<FactoryTargetDeclaration>.Empty, InterfaceMembers: ImmutableArray<MemberDeclaration>.Empty),
                    (innerFactory, resolvedRootFactoryMethod) =>
                    {
                        var result = this.GenerateFactoryMethod(
                            resolvedRootFactoryMethod.InjectionTree,
                            resolvedRootFactoryMethod,
                            innerFactory.factoryImplementation);
                        return (result.FactoryImplementation, innerFactory.DefaultCreateMethods.Add(result.DefaultCreateMethod), innerFactory.InterfaceMembers.AddRange(result.InterfaceMembers));
                    });

                var interfaceDeclarations = factory.InterfaceDeclarations;
                var interfaceInterfaces = factoryResolvedGraph.FactoryInterfaceType == pair.Key ? interfaces : ImmutableList<NamedType>.Empty;
                if (factoryResolvedGraph.FactoryType != pair.Key)
                {
                    interfaceDeclarations = interfaceDeclarations.Add(
                        new InterfaceDeclaration(
                            pair.Key,
                            interfaceInterfaces,
                            ImmutableArray.Create(knownSyntax.FactoryAttribute(
                                defaultMethodDeclarations.SelectMany(x => x.BindableFactoryTargets).DistinctInOrder())),
                            interfaceMembers.DistinctInOrder().ToArray()));
                }

                return (factoryImplementation,
                    interfaceDeclarations,
                    factory.DefaultCreateMethods.AddRange(defaultMethodDeclarations));
            });

        if (factoryResolvedGraph.FactoryInterfaceType != null)
        {
            interfaces = ImmutableList.Create(factoryResolvedGraph.FactoryInterfaceType);
        }

        var constructorMethodDeclaration = factoryResolvedGraph.DeclaredConstructor.IsPartialDefinition
            ? Member._MethodImplementation(
                    new MethodDeclaration(
                        factoryResolvedGraph.DeclaredConstructor.IsPublic ? DeclaredAccessibility.Public : DeclaredAccessibility.Private,
                        true,
                        true,
                        false,
                        "Constructor",
                        factoryImplementation.Constructor.Parameters.Select(x => x with
                        {
                            ParameterNecessity = x.ParameterNecessity switch
                            {
                                ParameterNecessity.Optional optional => optional with { HasDefaultValue = false },
                                ParameterNecessity.Required required => required,
                            },
                        }).ToValueList(),
                        new UsedType(factoryResolvedGraph.FactoryType, false)),
                    Statement.ReturnStatement(Expression._ConstructorCall(
                        factoryResolvedGraph.FactoryType,
                        factoryImplementation.Constructor.Parameters.Select(x =>
                            {
                                var expression = Expression.Identifier(x.Name);
                                if (x.ParameterNecessity.IsOptional && x.Type.IsValueType)
                                {
                                    expression = Expression.InvocationExpression(Expression.MemberAccessExpression(expression, "GetValueOrDefault"), []);
                                }

                                return expression;
                            })
                        .ToArray())).ToReadOnlyList())
                .ToEnumerable()
            : [];

        var classDeclaration =
            new ClassDeclaration(
                factoryResolvedGraph.FactoryType,
                !factoryImplementation.FactoryMethods.Any(),
                factoryImplementation.Fields.Select(x => new Member.Field(x))
                    .Concat(
                        new Member.MethodImplementation(
                            new MethodDeclaration(DeclaredAccessibility.Public, false, false, false, factoryResolvedGraph.FactoryType.Name, factoryImplementation.Constructor.Parameters),
                            factoryImplementation.Constructor.Statements).ToEnumerable<Member>(),
                        factoryImplementation.RootFactoryProperties.Select(x => Member._PropertyImplementation(x.Declaration, x.GetPropertyImplementation)),
                        factoryImplementation.RootFactoryMethods.Select(x =>
                            new Member.MethodImplementation(x.Declaration, x.MethodImplementation.Statements)),
                        factoryImplementation.DisposeMethodImplementations.Select(x =>
                            new Member.MethodImplementation(x.Declaration, x.Statements)),
                        disposeMethods,
                        factoryImplementation.FactoryMethods.Select(x =>
                            new Member.MethodImplementation(x.Declaration, x.MethodImplementation.Statements)),
                        factoryImplementation.PrivateCreateMethods.Select(x =>
                            new Member.MethodImplementation(x.Declaration, x.MethodImplementation.Statements)),
                        constructorMethodDeclaration)
                    .ToArray(),
                ImmutableArray.Create(knownSyntax.FactoryAttribute(defaultMethodDeclarations.SelectMany(x => x.BindableFactoryTargets))),
                interfaces);
        return new FactoryDeclarations(classDeclaration, interfaceDeclarations, defaultMethodDeclarations);
    }

    private (FactoryImplementation FactoryImplementation, FactoryTargetDeclaration DefaultCreateMethod, ImmutableArray<MemberDeclaration> InterfaceMembers) GenerateFactoryMethod(
        InjectionNode injectionNode,
        ResolvedRootFactoryMethod resolvedRootFactoryMethod,
        in FactoryImplementation factoryImplementation)
    {
        var factoryNode = this.generatorFeatures.InjectionNodeExpressionGenerator.Generate(
            injectionNode,
            in factoryImplementation,
            new MethodImplementation());
        var targetTypeParameterName = NameHelper.GetIdentifierNameForType(resolvedRootFactoryMethod.Target.Type);

        var fields = factoryNode.FactoryImplementation.Fields;

        var disposeMethodImplementations = factoryImplementation.DisposeMethodImplementations;
        var rootFactoryMethodStatements = factoryNode.RootFactoryMethod.Statements;
        var asyncRootFactoryMethodReturnType = compilationData.TaskType.ToClosedGenericType(ImmutableArray.Create(new FullTypeArgument(resolvedRootFactoryMethod.Return)));
        var constructorStatements = factoryNode.FactoryImplementation.Constructor.Statements;
        var rootFactoryProperties = factoryNode.FactoryImplementation.RootFactoryProperties;
        var rootFactoryMethods = factoryNode.FactoryImplementation.RootFactoryMethods;
        var rootInterfaceMembers = factoryNode.FactoryImplementation.RootInterfaceMembers;

        var interfaceMembers = ImmutableArray<MemberDeclaration>.Empty;
        var factoryDefinition = new FactoryDefinition(
            constructorStatements,
            factoryNode.FactoryImplementation.Fields,
            rootFactoryProperties,
            rootFactoryMethods,
            rootFactoryMethodStatements,
            rootInterfaceMembers,
            disposeMethodImplementations,
            interfaceMembers);

        var rootFactoryMethodParameters = resolvedRootFactoryMethod.Parameters
            .Select(x => new ParameterDeclaration(x.Type, x.Name, x.ParameterNecessity))
            .ToImmutableList();

        var rootFactoryMethodDeclaration = new MethodDeclaration(
            DeclaredAccessibility.Public,
            false,
            resolvedRootFactoryMethod.IsPartialDefinition,
            false,
            resolvedRootFactoryMethod.Name,
            rootFactoryMethodParameters,
            new UsedType(resolvedRootFactoryMethod.Return.Type));

        var isSingleton = resolvedRootFactoryMethod.InjectionTree is SingleInstancePerFactoryInjectionNode;
        var bindableFactoryTargets = ImmutableArray.CreateBuilder<string>();

        var factoryInput = new FactoryInput(rootFactoryMethodDeclaration, asyncRootFactoryMethodReturnType, targetTypeParameterName, isSingleton);
        factoryDefinition = resolvedRootFactoryMethod.RootLifecycle switch
        {
            Lifecycle.None => this.AddCreationWithoutLifecycle(resolvedRootFactoryMethod, factoryNode, factoryInput, factoryDefinition, bindableFactoryTargets),
            Lifecycle.Initialization => this.AddCreationWithInitialization(resolvedRootFactoryMethod, factoryNode, factoryInput, factoryDefinition, bindableFactoryTargets),
            Lifecycle.Disposal => this.AddCreationWithDisposal(resolvedRootFactoryMethod, factoryNode, factoryInput, factoryDefinition, bindableFactoryTargets),
            Lifecycle.Both => AddCreationWithInitializationAndDisposal(resolvedRootFactoryMethod, factoryNode, factoryInput, factoryDefinition, bindableFactoryTargets),
        };

        if (resolvedRootFactoryMethod.RootLifecycle != Lifecycle.None)
        {
            if (resolvedRootFactoryMethod.IsProperty)
            {
                var rootFactoryPropertyDeclaration = new PropertyDeclaration(
                    resolvedRootFactoryMethod.Return.Type,
                    resolvedRootFactoryMethod.Name,
                    resolvedRootFactoryMethod.IsPartialDefinition,
                    ImmutableList.Create(knownSyntax.BindableFactoryTargetAttribute));

                bindableFactoryTargets.Add(resolvedRootFactoryMethod.Name);

                rootFactoryMethodStatements = rootFactoryMethodStatements.Add(Statement.ExpressionStatement(
                    new InvocationExpression(knownSyntax.SharedLifecycleHandler.InitializeMethod)));
                rootFactoryMethodStatements =
                    rootFactoryMethodStatements.Add(
                        new ReturnStatement(factoryNode.DependantArguments.Single()));
                rootFactoryProperties = rootFactoryProperties
                    .Add(new DeclaredPropertyImplementation(
                        rootFactoryPropertyDeclaration,
                        rootFactoryMethodStatements));

                if (resolvedRootFactoryMethod.IsPartialDefinition)
                {
                    rootInterfaceMembers = rootInterfaceMembers.Add(rootFactoryPropertyDeclaration);
                    interfaceMembers = interfaceMembers.Add(rootFactoryPropertyDeclaration);
                }

                bindableFactoryTargets.Add(rootFactoryPropertyDeclaration.Name);
            }
            else
            {
                if (isSingleton)
                {
                    rootFactoryMethodStatements = rootFactoryMethodStatements.Add(Statement.ExpressionStatement(
                        new InvocationExpression(knownSyntax.SharedLifecycleHandler.InitializeMethod)));
                    rootFactoryMethodStatements =
                        rootFactoryMethodStatements.Add(
                            new ReturnStatement(factoryNode.DependantArguments.Single()));
                    rootFactoryMethods = rootFactoryMethods
                        .Add(new DeclaredMethodImplementation(
                            rootFactoryMethodDeclaration,
                            new MethodImplementation(ImmutableList<Declaration>.Empty, rootFactoryMethodStatements)));

                    if (resolvedRootFactoryMethod.IsPartialDefinition)
                    {
                        rootInterfaceMembers = rootInterfaceMembers.Add(rootFactoryMethodDeclaration);
                        interfaceMembers = interfaceMembers.Add(rootFactoryMethodDeclaration);
                    }

                    bindableFactoryTargets.Add(rootFactoryMethodDeclaration.Name);
                }
                else
                {
                    rootFactoryMethodStatements = rootFactoryMethodStatements.Insert(0, knownSyntax.SharedLifecycleHandler.CreateChildLifecycleHandlerAndAssignVarStatement);

                    var constructedValueVariableName = injectionNode.GetInjectionNodeName().Uncapitalize() + Result;
                    var constructedValueIdentifier = new Identifier(constructedValueVariableName);
                    rootFactoryMethodStatements = rootFactoryMethodStatements.Add(
                            new LocalDeclarationStatement(
                                constructedValueVariableName,
                                factoryNode.DependantArguments.Single()))
                        .Add(
                            new ExpressionStatement(
                                new InvocationExpression(
                                    knownSyntax.SharedLifecycleHandler.TryAddMethod,
                                    [constructedValueIdentifier, knownSyntax.ChildLifecycleHandler.Access,])));

                    var constructedType = compilationData.ReferencedSundewInjectionCompilationData.ConstructedType.ToClosedGenericType(
                            ImmutableArray.Create(new FullTypeArgument(resolvedRootFactoryMethod.Return)));
                    var createMethodAsyncDeclaration = new MethodDeclaration(
                        DeclaredAccessibility.Public,
                        false,
                        false,
                        false,
                        true,
                        resolvedRootFactoryMethod.Name + Trivia.AsyncName,
                        rootFactoryMethodParameters,
                        ImmutableArray.Create(knownSyntax.IndirectCreateMethodAttribute),
                        new UsedType(asyncRootFactoryMethodReturnType));
                    var createUnInitializedMethodDeclaration = new MethodDeclaration(
                        DeclaredAccessibility.Public,
                        false,
                        false,
                        false,
                        false,
                        resolvedRootFactoryMethod.Name + Uninitialized,
                        rootFactoryMethodParameters,
                        ImmutableArray.Create(knownSyntax.EditorBrowsableAttribute, knownSyntax.BindableFactoryTargetAttribute, knownSyntax.IndirectCreateMethodAttribute),
                        new UsedType(constructedType));

                    bindableFactoryTargets.Add(createUnInitializedMethodDeclaration.Name);
                    rootInterfaceMembers = rootInterfaceMembers.Add(createMethodAsyncDeclaration).Add(createUnInitializedMethodDeclaration);
                    interfaceMembers = interfaceMembers.Add(createMethodAsyncDeclaration).Add(createUnInitializedMethodDeclaration);

                    var createStatement =
                        new LocalDeclarationStatement(
                            Constructed + resolvedRootFactoryMethod.Target.Type.Name,
                            new InvocationExpression(
                                new MemberAccessExpression(Identifier.This, createUnInitializedMethodDeclaration.Name),
                                rootFactoryMethodParameters.Select(x => new Identifier(x.Name))
                                    .ToImmutableArray()));
                    rootFactoryMethods = rootFactoryMethods
                        .Add(new DeclaredMethodImplementation(rootFactoryMethodDeclaration, factoryNode.RootFactoryMethod with
                        {
                            Statements = ImmutableList.Create<Statement>(createStatement)
                                .Add(Statement.ExpressionStatement(
                                    new InvocationExpression(knownSyntax.SharedLifecycleHandler.InitializeMethod)))
                                .Add(new ReturnStatement(new MemberAccessExpression(new Identifier(createStatement.Name), ObjectPropertyName))),
                        }))
                        .Add(new DeclaredMethodImplementation(createMethodAsyncDeclaration, factoryNode.RootFactoryMethod with
                        {
                            Statements = ImmutableList.Create<Statement>(createStatement)
                                .Add(Statement.ExpressionStatement(knownSyntax.SharedLifecycleHandler
                                    .InitializeAsyncMethodCall))
                                .Add(new ReturnStatement(new MemberAccessExpression(new Identifier(createStatement.Name), ObjectPropertyName))),
                        }));
                    rootFactoryMethods = rootFactoryMethods.Add(new DeclaredMethodImplementation(
                        createUnInitializedMethodDeclaration,
                        factoryNode.RootFactoryMethod with
                        {
                            Statements = rootFactoryMethodStatements.Add(new ReturnStatement(
                                CreationExpression._ConstructorCall(constructedType, ImmutableArray.Create(new Identifier(constructedValueVariableName), knownSyntax.ChildLifecycleHandler.Access)))),
                        }));

                    var disposeForMethodDeclaration = new MethodDeclaration(
                        DeclaredAccessibility.Public,
                        false,
                        false,
                        false,
                        knownSyntax.DisposeName,
                        ImmutableList.Create(new ParameterDeclaration(resolvedRootFactoryMethod.Return.Type, targetTypeParameterName, ParameterNecessity._Required)),
                        new UsedType(compilationData.VoidType));
                    disposeMethodImplementations = disposeMethodImplementations.Add(
                        new DeclaredDisposeMethodImplementation(
                            disposeForMethodDeclaration,
                            ImmutableList.Create<Statement>(new ExpressionStatement(
                                new InvocationExpression(
                                    knownSyntax.SharedLifecycleHandler.DisposeMethod,
                                    [new Identifier(targetTypeParameterName)])))));
                    interfaceMembers = interfaceMembers.Add(disposeForMethodDeclaration);

                    var disposeForAsyncMethodDeclaration = new MethodDeclaration(
                        DeclaredAccessibility.Public,
                        false,
                        false,
                        false,
                        knownSyntax.DisposeAsyncName,
                        ImmutableList.Create(new ParameterDeclaration(resolvedRootFactoryMethod.Return.Type, targetTypeParameterName, ParameterNecessity._Required)),
                        new UsedType(compilationData.ValueTaskType));
                    disposeMethodImplementations = disposeMethodImplementations.Add(
                        new DeclaredDisposeMethodImplementation(
                            disposeForAsyncMethodDeclaration,
                            ImmutableList.Create<Statement>(new ReturnStatement(
                                new InvocationExpression(
                                    knownSyntax.SharedLifecycleHandler.DisposeAsyncMethod,
                                    [new Identifier(targetTypeParameterName)])))));
                    interfaceMembers = interfaceMembers.Add(disposeForAsyncMethodDeclaration);
                }
            }
        }
        else
        {
            if (resolvedRootFactoryMethod.IsProperty)
            {
                bindableFactoryTargets.Add(resolvedRootFactoryMethod.Name);

                if (resolvedRootFactoryMethod.IsPartialDefinition)
                {
                    var rootFactoryPropertyDeclaration = new PropertyDeclaration(
                        resolvedRootFactoryMethod.Return.Type,
                        resolvedRootFactoryMethod.Name,
                        resolvedRootFactoryMethod.IsPartialDefinition,
                        ImmutableList.Create(knownSyntax.BindableFactoryTargetAttribute));

                    rootFactoryMethodStatements =
                        rootFactoryMethodStatements.Add(
                            new ReturnStatement(factoryNode.DependantArguments.Single()));
                    rootFactoryProperties = rootFactoryProperties
                        .Add(new DeclaredPropertyImplementation(
                            rootFactoryPropertyDeclaration,
                            rootFactoryMethodStatements));

                    if (resolvedRootFactoryMethod.IsPartialDefinition)
                    {
                        rootInterfaceMembers = rootInterfaceMembers.Add(rootFactoryPropertyDeclaration);
                        interfaceMembers = interfaceMembers.Add(rootFactoryPropertyDeclaration);
                    }
                }
                else
                {
                    constructorStatements = constructorStatements.Add(Statement.ExpressionStatement(Expression.AssignmentExpression(Expression.MemberAccessExpression(Identifier.This, resolvedRootFactoryMethod.Name), factoryNode.DependantArguments.Single())));
                }
            }
            else
            {
                rootFactoryMethodStatements =
                    rootFactoryMethodStatements.Add(
                        new ReturnStatement(factoryNode.DependantArguments.Single()));
                rootFactoryMethods = rootFactoryMethods
                    .Add(new DeclaredMethodImplementation(
                        rootFactoryMethodDeclaration with { Attributes = ImmutableList.Create(knownSyntax.BindableFactoryTargetAttribute), }, factoryNode.RootFactoryMethod with { Statements = rootFactoryMethodStatements }));

                bindableFactoryTargets.Add(rootFactoryMethodDeclaration.Name);

                if (resolvedRootFactoryMethod.IsPartialDefinition)
                {
                    rootInterfaceMembers = rootInterfaceMembers.Add(rootFactoryMethodDeclaration);
                    interfaceMembers = interfaceMembers.Add(rootFactoryMethodDeclaration);
                }
            }
        }

        return (factoryNode.FactoryImplementation with
        {
            Constructor = factoryNode.FactoryImplementation.Constructor with { Statements = constructorStatements },
            Fields = fields,
            RootFactoryProperties = rootFactoryProperties,
            RootFactoryMethods = rootFactoryMethods,
            RootInterfaceMembers = rootInterfaceMembers,
            DisposeMethodImplementations = disposeMethodImplementations,
        },
            new FactoryTargetDeclaration(rootFactoryMethodDeclaration.Name, rootFactoryMethodDeclaration.Parameters, resolvedRootFactoryMethod.Return.Type, resolvedRootFactoryMethod.IsProperty, bindableFactoryTargets.ToImmutable()),
            interfaceMembers);
    }

    private FactoryDefinition AddCreationWithInitialization(
        ResolvedRootFactoryMethod resolvedRootFactoryMethod,
        FactoryNode factoryNode,
        FactoryInput factoryInput,
        FactoryDefinition factoryDefinition,
        ImmutableArray<string>.Builder bindableFactoryTargets)
    {
        if (resolvedRootFactoryMethod.IsProperty)
        {
            var rootFactoryPropertyDeclaration = new PropertyDeclaration(
                resolvedRootFactoryMethod.Return.Type,
                resolvedRootFactoryMethod.Name,
                resolvedRootFactoryMethod.IsPartialDefinition,
                ImmutableList.Create(knownSyntax.BindableFactoryTargetAttribute));

            bindableFactoryTargets.Add(resolvedRootFactoryMethod.Name);

            rootFactoryMethodStatements = rootFactoryMethodStatements.Add(Statement.ExpressionStatement(
                new InvocationExpression(knownSyntax.SharedLifecycleHandler.InitializeMethod)));
            rootFactoryMethodStatements =
                rootFactoryMethodStatements.Add(
                    new ReturnStatement(factoryNode.DependantArguments.Single()));
            rootFactoryProperties = rootFactoryProperties
                .Add(new DeclaredPropertyImplementation(
                    rootFactoryPropertyDeclaration,
                    rootFactoryMethodStatements));

            if (resolvedRootFactoryMethod.IsPartialDefinition)
            {
                rootInterfaceMembers = rootInterfaceMembers.Add(rootFactoryPropertyDeclaration);
                interfaceMembers = interfaceMembers.Add(rootFactoryPropertyDeclaration);
            }

            bindableFactoryTargets.Add(rootFactoryPropertyDeclaration.Name);
        }
        else
        {
            if (isSingleton)
            {
                rootFactoryMethodStatements = rootFactoryMethodStatements.Add(Statement.ExpressionStatement(
                    new InvocationExpression(knownSyntax.SharedLifecycleHandler.InitializeMethod)));
                rootFactoryMethodStatements =
                    rootFactoryMethodStatements.Add(
                        new ReturnStatement(factoryNode.DependantArguments.Single()));
                rootFactoryMethods = rootFactoryMethods
                    .Add(new DeclaredMethodImplementation(
                        rootFactoryMethodDeclaration,
                        new MethodImplementation(ImmutableList<Declaration>.Empty, rootFactoryMethodStatements)));

                if (resolvedRootFactoryMethod.IsPartialDefinition)
                {
                    rootInterfaceMembers = rootInterfaceMembers.Add(rootFactoryMethodDeclaration);
                    interfaceMembers = interfaceMembers.Add(rootFactoryMethodDeclaration);
                }

                bindableFactoryTargets.Add(rootFactoryMethodDeclaration.Name);
            }
            else
            {
                rootFactoryMethodStatements = rootFactoryMethodStatements.Insert(
                    0,
                    knownSyntax.SharedLifecycleHandler.CreateChildLifecycleHandlerAndAssignVarStatement);

                var constructedValueVariableName = injectionNode.GetInjectionNodeName().Uncapitalize() + Result;
                var constructedValueIdentifier = new Identifier(constructedValueVariableName);
                rootFactoryMethodStatements = rootFactoryMethodStatements.Add(
                        new LocalDeclarationStatement(
                            constructedValueVariableName,
                            factoryNode.DependantArguments.Single()))
                    .Add(
                        new ExpressionStatement(
                            new InvocationExpression(
                                knownSyntax.SharedLifecycleHandler.TryAddMethod,
                                [constructedValueIdentifier, knownSyntax.ChildLifecycleHandler.Access,])));

                var constructedType = compilationData.ReferencedSundewInjectionCompilationData.ConstructedType
                    .ToClosedGenericType(
                        ImmutableArray.Create(new FullTypeArgument(resolvedRootFactoryMethod.Return)));
                var createMethodAsyncDeclaration = new MethodDeclaration(
                    DeclaredAccessibility.Public,
                    false,
                    false,
                    false,
                    true,
                    resolvedRootFactoryMethod.Name + Trivia.AsyncName,
                    rootFactoryMethodParameters,
                    ImmutableArray.Create(knownSyntax.IndirectCreateMethodAttribute),
                    new UsedType(asyncRootFactoryMethodReturnType));
                var createUnInitializedMethodDeclaration = new MethodDeclaration(
                    DeclaredAccessibility.Public,
                    false,
                    false,
                    false,
                    false,
                    resolvedRootFactoryMethod.Name + Uninitialized,
                    rootFactoryMethodParameters,
                    ImmutableArray.Create(
                        knownSyntax.EditorBrowsableAttribute,
                        knownSyntax.BindableFactoryTargetAttribute,
                        knownSyntax.IndirectCreateMethodAttribute),
                    new UsedType(constructedType));

                bindableFactoryTargets.Add(createUnInitializedMethodDeclaration.Name);
                rootInterfaceMembers = rootInterfaceMembers.Add(createMethodAsyncDeclaration)
                    .Add(createUnInitializedMethodDeclaration);
                interfaceMembers = interfaceMembers.Add(createMethodAsyncDeclaration)
                    .Add(createUnInitializedMethodDeclaration);

                var createStatement =
                    new LocalDeclarationStatement(
                        Constructed + resolvedRootFactoryMethod.Target.Type.Name,
                        new InvocationExpression(
                            new MemberAccessExpression(Identifier.This, createUnInitializedMethodDeclaration.Name),
                            rootFactoryMethodParameters.Select(x => new Identifier(x.Name))
                                .ToImmutableArray()));
                rootFactoryMethods = rootFactoryMethods
                    .Add(new DeclaredMethodImplementation(
                        rootFactoryMethodDeclaration,
                        factoryNode.RootFactoryMethod with
                        {
                            Statements = ImmutableList.Create<Statement>(createStatement)
                                .Add(Statement.ExpressionStatement(
                                    new InvocationExpression(knownSyntax.SharedLifecycleHandler.InitializeMethod)))
                                .Add(new ReturnStatement(
                                    new MemberAccessExpression(new Identifier(createStatement.Name), ObjectPropertyName))),
                        }))
                    .Add(new DeclaredMethodImplementation(
                        createMethodAsyncDeclaration,
                        factoryNode.RootFactoryMethod with
                        {
                            Statements = ImmutableList.Create<Statement>(createStatement)
                                .Add(Statement.ExpressionStatement(knownSyntax.SharedLifecycleHandler
                                    .InitializeAsyncMethodCall))
                                .Add(new ReturnStatement(
                                    new MemberAccessExpression(
                                        new Identifier(createStatement.Name),
                                        ObjectPropertyName))),
                        }));
                rootFactoryMethods = rootFactoryMethods.Add(new DeclaredMethodImplementation(
                    createUnInitializedMethodDeclaration,
                    factoryNode.RootFactoryMethod with
                    {
                        Statements = rootFactoryMethodStatements.Add(new ReturnStatement(
                            CreationExpression._ConstructorCall(
                                constructedType,
                                ImmutableArray.Create(
                                    new Identifier(constructedValueVariableName),
                                    knownSyntax.ChildLifecycleHandler.Access)))),
                    }));
            }
        }

        return factoryDefinition;
    }

    private FactoryDefinition AddCreationWithDisposal(
        ResolvedRootFactoryMethod resolvedRootFactoryMethod,
        FactoryNode factoryNode,
        FactoryInput factoryInput,
        FactoryDefinition factoryDefinition,
        ImmutableArray<string>.Builder bindableFactoryTargets)
    {
        factoryDefinition = this.AddCreationWithoutLifecycle(resolvedRootFactoryMethod, factoryNode, factoryInput, factoryDefinition, bindableFactoryTargets);
        return this.AddDisposal(resolvedRootFactoryMethod, factoryInput, factoryDefinition);
    }

    private FactoryDefinition AddCreationWithoutLifecycle(
            ResolvedRootFactoryMethod resolvedRootFactoryMethod,
            FactoryNode factoryNode,
            FactoryInput factoryInput,
            FactoryDefinition factoryDefinition,
            ImmutableArray<string>.Builder bindableFactoryTargets)
    {
        if (resolvedRootFactoryMethod.IsProperty)
        {
            bindableFactoryTargets.Add(resolvedRootFactoryMethod.Name);

            if (resolvedRootFactoryMethod.IsPartialDefinition)
            {
                var rootFactoryPropertyDeclaration = new PropertyDeclaration(
                    resolvedRootFactoryMethod.Return.Type,
                    resolvedRootFactoryMethod.Name,
                    resolvedRootFactoryMethod.IsPartialDefinition,
                    ImmutableList.Create(knownSyntax.BindableFactoryTargetAttribute));

                factoryDefinition.RootFactoryMethodStatements =
                    factoryDefinition.RootFactoryMethodStatements.Add(
                        new ReturnStatement(factoryNode.DependantArguments.Single()));
                factoryDefinition.RootFactoryProperties = factoryDefinition.RootFactoryProperties
                    .Add(new DeclaredPropertyImplementation(
                        rootFactoryPropertyDeclaration,
                        factoryDefinition.RootFactoryMethodStatements));

                if (resolvedRootFactoryMethod.IsPartialDefinition)
                {
                    factoryDefinition.RootInterfaceMembers = factoryDefinition.RootInterfaceMembers.Add(rootFactoryPropertyDeclaration);
                    factoryDefinition.InterfaceMembers = factoryDefinition.InterfaceMembers.Add(rootFactoryPropertyDeclaration);
                }
            }
            else
            {
                factoryDefinition.FactoryConstructorStatements = factoryDefinition.FactoryConstructorStatements.Add(Statement.ExpressionStatement(Expression.AssignmentExpression(Expression.MemberAccessExpression(Identifier.This, resolvedRootFactoryMethod.Name), factoryNode.DependantArguments.Single())));
            }
        }
        else
        {
            factoryDefinition.RootFactoryMethodStatements =
                factoryDefinition.RootFactoryMethodStatements.Add(
                    new ReturnStatement(factoryNode.DependantArguments.Single()));
            factoryDefinition.RootFactoryMethods = factoryDefinition.RootFactoryMethods
                .Add(new DeclaredMethodImplementation(
                    factoryInput.RootFactoryMethodDeclaration with { Attributes = ImmutableList.Create(knownSyntax.BindableFactoryTargetAttribute), }, factoryNode.RootFactoryMethod with { Statements = factoryDefinition.RootFactoryMethodStatements }));

            bindableFactoryTargets.Add(factoryInput.RootFactoryMethodDeclaration.Name);

            if (resolvedRootFactoryMethod.IsPartialDefinition)
            {
                factoryDefinition.RootInterfaceMembers = factoryDefinition.RootInterfaceMembers.Add(factoryInput.RootFactoryMethodDeclaration);
                factoryDefinition.InterfaceMembers = factoryDefinition.InterfaceMembers.Add(factoryInput.RootFactoryMethodDeclaration);
            }
        }

        return factoryDefinition;
    }

    private FactoryDefinition AddDisposal(ResolvedRootFactoryMethod resolvedRootFactoryMethod, FactoryInput factoryInput, FactoryDefinition factoryDefinition)
    {
        var disposeForMethodDeclaration = new MethodDeclaration(
            DeclaredAccessibility.Public,
            false,
            false,
            false,
            knownSyntax.DisposeName,
            ImmutableList.Create(new ParameterDeclaration(resolvedRootFactoryMethod.Return.Type, factoryInput.TargetTypeParameterName, ParameterNecessity._Required)),
            new UsedType(compilationData.VoidType));

        factoryDefinition.DisposeMethods = factoryDefinition.DisposeMethods.Add(
            new DeclaredDisposeMethodImplementation(
                disposeForMethodDeclaration,
                ImmutableList.Create<Statement>(new ExpressionStatement(
                    new InvocationExpression(
                        knownSyntax.SharedLifecycleHandler.DisposeMethod,
                        [new Identifier(factoryInput.TargetTypeParameterName)])))));
        factoryDefinition.InterfaceMembers = factoryDefinition.InterfaceMembers.Add(disposeForMethodDeclaration);

        var disposeForAsyncMethodDeclaration = new MethodDeclaration(
            DeclaredAccessibility.Public,
            false,
            false,
            false,
            knownSyntax.DisposeAsyncName,
            ImmutableList.Create(new ParameterDeclaration(resolvedRootFactoryMethod.Return.Type, factoryInput.TargetTypeParameterName, ParameterNecessity._Required)),
            new UsedType(compilationData.ValueTaskType));
        factoryDefinition.DisposeMethods = factoryDefinition.DisposeMethods.Add(
            new DeclaredDisposeMethodImplementation(
                disposeForAsyncMethodDeclaration,
                ImmutableList.Create<Statement>(new ReturnStatement(
                    new InvocationExpression(
                        knownSyntax.SharedLifecycleHandler.DisposeAsyncMethod,
                        [new Identifier(factoryInput.TargetTypeParameterName)])))));
        factoryDefinition.InterfaceMembers = factoryDefinition.InterfaceMembers.Add(disposeForAsyncMethodDeclaration);
        return factoryDefinition;
    }

    private ref struct FactoryDefinition(
        ImmutableList<Statement> factoryConstructorStatements,
        ImmutableList<FieldDeclaration> fields,
        ImmutableList<DeclaredPropertyImplementation> rootFactoryProperties,
        ImmutableList<DeclaredMethodImplementation> rootFactoryMethods,
        ImmutableList<Statement> rootFactoryMethodStatements,
        ImmutableList<MemberDeclaration> rootInterfaceMembers,
        ImmutableList<DeclaredDisposeMethodImplementation> disposeMethods,
        ImmutableArray<MemberDeclaration> interfaceMembers)
    {
        public ImmutableList<Statement> FactoryConstructorStatements { get; set; } = factoryConstructorStatements;

        public ImmutableList<FieldDeclaration> Fields { get; set; } = fields;

        public ImmutableList<DeclaredPropertyImplementation> RootFactoryProperties { get; set; } = rootFactoryProperties;

        public ImmutableList<DeclaredMethodImplementation> RootFactoryMethods { get; set; } = rootFactoryMethods;

        public ImmutableList<Statement> RootFactoryMethodStatements { get; set; } = rootFactoryMethodStatements;

        public ImmutableList<MemberDeclaration> RootInterfaceMembers { get; set; } = rootInterfaceMembers;

        public ImmutableList<DeclaredDisposeMethodImplementation> DisposeMethods { get; set; } = disposeMethods;

        public ImmutableArray<MemberDeclaration> InterfaceMembers { get; set; } = interfaceMembers;
    }

    private readonly ref struct FactoryInput(
        MethodDeclaration rootFactoryMethodDeclaration,
        ClosedGenericType asyncRootFactoryMethodReturnType,
        string targetTypeParameterName,
        bool isSingleton)
    {
        public MethodDeclaration RootFactoryMethodDeclaration { get; } = rootFactoryMethodDeclaration;

        public ClosedGenericType AsyncRootFactoryMethodReturnType { get; } = asyncRootFactoryMethodReturnType;

        public string TargetTypeParameterName { get; } = targetTypeParameterName;

        public bool IsSingleton { get; } = isSingleton;
    }
}