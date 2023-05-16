// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactorySyntaxGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using Sundew.Base.Collections;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Templates;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage;
using Sundew.Injection.Generator.TypeSystem;
using InjectionNode = Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes.InjectionNode;
using MethodImplementation = Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.MethodImplementation;

internal class FactorySyntaxGenerator
{
    private const string ObjectPropertyName = "Object";
    private const string Uninitialized = "Uninitialized";
    private const string Constructed = "constructed";

    private readonly KnownSyntax knownSyntax;
    private readonly InjectionNodeEvaluator injectionNodeEvaluator;
    private readonly CompilationData compilationData;

    public FactorySyntaxGenerator(
        CompilationData compilationData,
        KnownSyntax knownSyntax,
        CancellationToken cancellationToken)
    {
        this.compilationData = compilationData;
        this.knownSyntax = knownSyntax;
        this.injectionNodeEvaluator = new InjectionNodeEvaluator(this.knownSyntax, this.compilationData, cancellationToken);
    }

    public FactoryDeclarations Generate(FactoryData factoryData)
    {
        var factoryImplementation = factoryData.FactoryMethodInfos.Aggregate(
            new FactoryImplementation(),
            (factory, factoryMethodInfo) => this.GenerateFactoryMethod(
                factoryMethodInfo.InjectionTree,
                factoryData,
                factoryMethodInfo,
                factory));

        var interfaces = ImmutableList<DefiniteType>.Empty;
        var disposeMethods = ImmutableList<Model.Syntax.MethodImplementation>.Empty;
        var fields = factoryImplementation.Fields;
        var constructorStatements = factoryImplementation.Constructor.Statements;
        if (factoryData.NeedsLifecycleHandling)
        {
            var (newFields, declaration) = fields.AddUnique(
                this.knownSyntax.SharedLifetimeHandler.AccessorName,
                this.knownSyntax.SharedLifetimeHandler.Type,
                (s, i) => s + i,
                name => new FieldDeclaration(this.compilationData.LifetimeHandlerType, name, null));
            constructorStatements = constructorStatements.Insert(0, this.knownSyntax.SharedLifetimeHandler.CreateLifetimeHandlerAndAssignFieldStatement);

            fields = newFields;

            interfaces = interfaces.Add(this.compilationData.IDisposableType)
                .Add(this.compilationData.IAsyncDisposableType);

            disposeMethods = disposeMethods.Add(new Model.Syntax.MethodImplementation(
                new MethodDeclaration(DeclaredAccessibility.Public, false, this.knownSyntax.DisposeName, ImmutableList<ParameterDeclaration>.Empty, this.compilationData.VoidType), ImmutableList.Create<Statement>(new ExpressionStatement(
                    new InvocationExpression(this.knownSyntax.SharedLifetimeHandler.DisposeMethod)))));

            disposeMethods = disposeMethods.Add(new Model.Syntax.MethodImplementation(
                new MethodDeclaration(DeclaredAccessibility.Public, false, this.knownSyntax.DisposeAsyncName, ImmutableList<ParameterDeclaration>.Empty, this.compilationData.ValueTaskType), ImmutableList.Create<Statement>(new ReturnStatement(
                    new InvocationExpression(this.knownSyntax.SharedLifetimeHandler.DisposeAsyncMethod)))));
        }

        InterfaceDeclaration? interfaceDeclaration = null;
        if (factoryData.GenerateInterface && factoryData.FactoryInterfaceType != null)
        {
            interfaceDeclaration = new InterfaceDeclaration(
                factoryData.FactoryInterfaceType,
                interfaces,
                factoryImplementation.CreateMethods.Select(x => x.Declaration)
                    .Concat(factoryImplementation.DisposeForMethodImplementations.Select(x => x.Declaration)).ToArray());
            interfaces = interfaces.Clear().Add(factoryData.FactoryInterfaceType);
        }

        var classDeclaration =
            new ClassDeclaration(
                factoryData.FactoryType,
                !factoryImplementation.FactoryMethods.Any(),
                fields.Select(x => new Field(x))
                    .Concat(
                        new Model.Syntax.MethodImplementation(
                            new MethodDeclaration(DeclaredAccessibility.Public, false, factoryData.FactoryType.Name, factoryImplementation.Constructor.Parameters),
                            constructorStatements).ToEnumerable<Member>(),
                        factoryImplementation.CreateMethods.Select(x =>
                            new Model.Syntax.MethodImplementation(x.Declaration, x.MethodImplementation.Statements)),
                        factoryImplementation.FactoryMethods.Select(x =>
                            new Model.Syntax.MethodImplementation(x.Declaration, x.MethodImplementation.Statements)),
                        factoryImplementation.DisposeForMethodImplementations.Select(x =>
                            new Model.Syntax.MethodImplementation(x.Declaration, x.Statements)),
                        disposeMethods,
                        factoryImplementation.PrivateCreateMethods.Select(x =>
                            new Model.Syntax.MethodImplementation(x.Declaration, x.MethodImplementation.Statements)))
                    .ToArray(),
                interfaces);
        return new FactoryDeclarations(interfaceDeclaration, classDeclaration);
    }

    private FactoryImplementation GenerateFactoryMethod(
        InjectionNode injectionNode,
        FactoryData factoryData,
        FactoryMethodData factoryMethodData,
        in FactoryImplementation factoryImplementation)
    {
        var factoryNode = this.injectionNodeEvaluator.Evaluate(
            injectionNode,
            factoryData,
            in factoryImplementation,
            new MethodImplementation());

        var targetTypeParameterName = NameHelper.GetVariableNameForType(factoryMethodData.Target.Type);

        var fields = factoryNode.FactoryImplementation.Fields;

        var disposeMethodImplementations = ImmutableList<DeclaredDisposeMethodImplementation>.Empty;
        var factoryMethodStatements = factoryNode.CreateMethod.Statements;
        var asyncCreateReturnType = this.compilationData.TaskType.ToDefiniteBoundGenericType(ImmutableArray.Create(new DefiniteTypeArgument(factoryMethodData.Return)));
        var factoryMethods = factoryImplementation.CreateMethods;
        var createMethodParameters = factoryNode.CreateMethod.Parameters.GroupBy(x => x.DefaultValue != null).OrderBy(x => x.Key).SelectMany(x => x).ToImmutableList();
        var factoryMethodDeclaration = new MethodDeclaration(
            DeclaredAccessibility.Public,
            false,
            factoryMethodData.FactoryMethodName,
            createMethodParameters,
            factoryMethodData.Return.Type);

        if (factoryData.NeedsLifecycleHandling)
        {
            factoryMethodStatements = factoryMethodStatements.Insert(0, this.knownSyntax.SharedLifetimeHandler.CreateChildLifetimeHandlerAndAssignVarStatement);

            var constructedValueVariableName = injectionNode.Name.Uncapitalize();
            var constructedValueIdentifier = new Identifier(constructedValueVariableName);
            factoryMethodStatements = factoryMethodStatements.Add(
                    new LocalDeclarationStatement(
                        constructedValueVariableName,
                        factoryNode.Arguments.Single()))
                .Add(
                    new ExpressionStatement(
                        new InvocationExpression(
                            this.knownSyntax.SharedLifetimeHandler.TryAddMethod,
                            new[] { constructedValueIdentifier, this.knownSyntax.ChildLifetimeHandler.Access, })));

            var constructedType =
                this.compilationData.ConstructedType.ToDefiniteBoundGenericType(
                    ImmutableArray.Create(new DefiniteTypeArgument(factoryMethodData.Return)));
            var factoryMethodAsyncDeclaration = new MethodDeclaration(
                DeclaredAccessibility.Public,
                false,
                true,
                factoryMethodData.FactoryMethodName + Trivia.AsyncName,
                createMethodParameters,
                ImmutableArray<AttributeDeclaration>.Empty,
                asyncCreateReturnType);
            var rawFactoryMethodDeclaration = new MethodDeclaration(
                DeclaredAccessibility.Public,
                false,
                false,
                factoryMethodData.FactoryMethodName + Uninitialized,
                createMethodParameters,
                ImmutableArray.Create(this.knownSyntax.EditorBrowsableAttribute),
                constructedType);
            var createStatement =
                new LocalDeclarationStatement(
                    Constructed + factoryMethodData.Target.Type.Name,
                    new InvocationExpression(
                        new MemberAccessExpression(Identifier.This, rawFactoryMethodDeclaration.Name),
                        createMethodParameters.Select(x => new Identifier(x.Name))
                            .ToImmutableArray()));
            factoryMethods = factoryMethods
                .Add(new DeclaredMethodImplementation(factoryMethodDeclaration, factoryNode.CreateMethod with
                {
                    Statements = ImmutableList.Create<Statement>(createStatement)
                        .Add(Statement.ExpressionStatement(new InvocationExpression(this.knownSyntax.SharedLifetimeHandler.InitializeMethod)))
                        .Add(new ReturnStatement(new MemberAccessExpression(new Identifier(createStatement.Name), ObjectPropertyName))),
                }))
                .Add(new DeclaredMethodImplementation(factoryMethodAsyncDeclaration, factoryNode.CreateMethod with
                {
                    Statements = ImmutableList.Create<Statement>(createStatement)
                        .Add(Statement.ExpressionStatement(this.knownSyntax.SharedLifetimeHandler.InitializeAsyncMethodCall))
                        .Add(new ReturnStatement(new MemberAccessExpression(new Identifier(createStatement.Name), ObjectPropertyName))),
                }));
            factoryMethods = factoryMethods.Add(new DeclaredMethodImplementation(
                rawFactoryMethodDeclaration,
                factoryNode.CreateMethod with
                {
                    Statements = factoryMethodStatements.Add(new ReturnStatement(new CreationExpression(CreationSource.ConstructorCall(constructedType), ImmutableArray.Create(new Identifier(constructedValueVariableName), this.knownSyntax.ChildLifetimeHandler.Access)))),
                }));

            disposeMethodImplementations = disposeMethodImplementations.Add(
                new DeclaredDisposeMethodImplementation(
                    new MethodDeclaration(
                        DeclaredAccessibility.Public,
                        false,
                        this.knownSyntax.DisposeName,
                        ImmutableList.Create(new ParameterDeclaration(factoryMethodData.Return.Type, targetTypeParameterName)),
                        this.compilationData.VoidType),
                    ImmutableList.Create<Statement>(new ExpressionStatement(
                        new InvocationExpression(
                            this.knownSyntax.SharedLifetimeHandler.DisposeMethod,
                            new Expression[] { new Identifier(targetTypeParameterName) })))));

            disposeMethodImplementations = disposeMethodImplementations.Add(
                new DeclaredDisposeMethodImplementation(
                    new MethodDeclaration(DeclaredAccessibility.Public, false, this.knownSyntax.DisposeAsyncName, ImmutableList.Create(new ParameterDeclaration(factoryMethodData.Return.Type, targetTypeParameterName)), this.compilationData.ValueTaskType),
                    ImmutableList.Create<Statement>(new ReturnStatement(
                        new InvocationExpression(
                            this.knownSyntax.SharedLifetimeHandler.DisposeAsyncMethod,
                            new Expression[] { new Identifier(targetTypeParameterName) })))));
        }
        else
        {
            factoryMethodStatements =
                factoryMethodStatements.Add(
                    new ReturnStatement(factoryNode.Arguments.Single()));
            factoryMethods = factoryMethods
                .Add(new DeclaredMethodImplementation(factoryMethodDeclaration, factoryNode.CreateMethod with { Statements = factoryMethodStatements }));
        }

        return factoryNode.FactoryImplementation with
        {
            Fields = fields,
            CreateMethods = factoryMethods,
            DisposeForMethodImplementations = disposeMethodImplementations,
        };
    }
}