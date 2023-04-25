// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactorySyntaxGenerator.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Sundew.Base.Collections;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage;
using Sundew.Injection.Generator.TypeSystem;
using InjectionNode = Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes.InjectionNode;
using MethodImplementation = Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.MethodImplementation;

internal class FactorySyntaxGenerator
{
    internal const string FactoryConstructorDisposingListName = "factoryConstructorDisposingList";
    internal const string FactoryMethodDisposingDictionaryName = "factoryMethodDisposingDictionary";
    internal const string LocalDisposingListName = "disposingList";
    internal const string DisposeMethodName = "Dispose";

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

        var disposeMethods = ImmutableList<Model.Syntax.MethodImplementation>.Empty;
        var interfaces = ImmutableList<DefiniteType>.Empty;
        if (factoryImplementation.DisposeMethodStatements.Any())
        {
            interfaces = interfaces.Add(this.compilationData.IDisposableType);
            disposeMethods = disposeMethods.Add(new Model.Syntax.MethodImplementation(
                new MethodDeclaration(DeclaredAccessibility.Public, false, DisposeMethodName, ImmutableList<ParameterDeclaration>.Empty, this.compilationData.VoidType), factoryImplementation.DisposeMethodStatements));
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
                factoryImplementation.Fields.Select(x => new Field(x))
                    .Concat(
                        new Model.Syntax.MethodImplementation(
                            new MethodDeclaration(DeclaredAccessibility.Public, false, factoryData.FactoryType.Name, factoryImplementation.Constructor.Parameters), factoryImplementation.Constructor.Statements).ToEnumerable<Member>(),
                        factoryImplementation.CreateMethods.Select(x => new Model.Syntax.MethodImplementation(x.Declaration, x.MethodImplementation.Statements)),
                        factoryImplementation.FactoryMethods.Select(x => new Model.Syntax.MethodImplementation(x.Declaration, x.MethodImplementation.Statements)),
                        factoryImplementation.DisposeForMethodImplementations.Select(x => new Model.Syntax.MethodImplementation(x.Declaration, x.Statements)),
                        disposeMethods).ToArray(),
                interfaces);
        return new FactoryDeclarations(interfaceDeclaration, classDeclaration);
    }

    private FactoryImplementation GenerateFactoryMethod(
        InjectionNode injectionNode,
        FactoryData factoryModel,
        FactoryMethodData factoryMethodData,
        in FactoryImplementation factoryImplementation)
    {
        var factoryNode = this.injectionNodeEvaluator.Evaluate(
            injectionNode,
            factoryModel,
            in factoryImplementation,
            new MethodImplementation());

        var disposeMethodStatements = ImmutableList<Statement>.Empty;
        var fields = factoryNode.FactoryImplementation.Fields;
        var factoryConstructorStatements = factoryNode.FactoryImplementation.Constructor.Statements;
        if (factoryNode.FactoryImplementation.Constructor.RequiresDisposableList)
        {
            var (newFields, declaration) = fields.AddUnique(
                FactoryConstructorDisposingListName,
                this.compilationData.DisposableListType,
                (s, i) => s + i,
                name => new FieldDeclaration(this.compilationData.DisposableListType, name, new CreationExpression(CreationSource.ConstructorCall(this.compilationData.DisposableListType), ImmutableList<Expression>.Empty)));

            disposeMethodStatements = disposeMethodStatements.Add(
                new ExpressionStatement(
                    new InvocationExpression(this.knownSyntax.FactoryConstructorDisposingListSyntax.DisposeMethod)));
            fields = newFields;
        }

        var disposeMethodImplementations = ImmutableList<DeclaredDisposeMethodImplementation>.Empty;
        var factoryMethodStatements = factoryNode.CreateMethod.Statements;
        if (factoryNode.CreateMethod.RequiresDisposingList)
        {
            factoryMethodStatements = factoryMethodStatements.Insert(0, Statement.LocalDeclarationStatement(LocalDisposingListName, new CreationExpression(CreationSource.ConstructorCall(this.compilationData.DisposableListType))));
            var weakKeyDisposingDictionaryType = this.compilationData.WeakKeyDisposingDictionary.ToDefiniteBoundGenericType(ImmutableArray.Create(new DefiniteTypeArgument(factoryMethodData.Return.Type, factoryMethodData.Return.TypeMetadata)));

            var (newFields, declaration) = fields.AddUnique(
                FactoryMethodDisposingDictionaryName,
                weakKeyDisposingDictionaryType,
                (s, i) => s + i,
                name => new FieldDeclaration(weakKeyDisposingDictionaryType, name, new CreationExpression(CreationSource.ConstructorCall(weakKeyDisposingDictionaryType), ImmutableList<Expression>.Empty)));
            fields = newFields;

            var returnValueVariableName = injectionNode.Name.Uncapitalize();
            var returnValueIdentifier = new Identifier(returnValueVariableName);
            factoryMethodStatements = factoryMethodStatements.Add(
                    new LocalDeclarationStatement(
                        returnValueVariableName,
                        factoryNode.Arguments.Single()))
                .Add(
                    new ExpressionStatement(
                        new InvocationExpression(
                            this.knownSyntax.FactoryMethodWeakKeyDisposingDictionarySyntax.TryAddMethod,
                            new[] { returnValueIdentifier, this.knownSyntax.LocalDisposingListSyntax.DisposableListAccess })))
                .Add(new ReturnStatement(returnValueIdentifier));

            disposeMethodStatements = disposeMethodStatements.Add(
                new ExpressionStatement(
                    new InvocationExpression(this.knownSyntax.FactoryMethodWeakKeyDisposingDictionarySyntax.DisposeMethod, Array.Empty<Expression>())));

            var disposeForParameterName = NameHelper.GetVariableNameForType(factoryMethodData.Target.Type);
            disposeMethodImplementations = disposeMethodImplementations.Add(
                new DeclaredDisposeMethodImplementation(
                    new MethodDeclaration(DeclaredAccessibility.Public, false, DisposeMethodName, ImmutableList.Create(new ParameterDeclaration(factoryMethodData.Return.Type, disposeForParameterName)), this.compilationData.VoidType),
                    ImmutableList.Create<Statement>(new ExpressionStatement(
                        new InvocationExpression(
                            this.knownSyntax.FactoryMethodWeakKeyDisposingDictionarySyntax.DisposeMethod,
                            new Expression[] { new Identifier(disposeForParameterName) })))));
        }
        else
        {
            factoryMethodStatements =
                factoryMethodStatements.Add(
                    new ReturnStatement(factoryNode.Arguments.Single()));
        }

        var factoryMethodDeclaration = new MethodDeclaration(
            DeclaredAccessibility.Public,
            false,
            factoryMethodData.FactoryMethodName,
            factoryNode.CreateMethod.Parameters,
            factoryMethodData.Return.Type);
        var factoryMethods = factoryImplementation.CreateMethods.Add(
            new DeclaredMethodImplementation(factoryMethodDeclaration, factoryNode.CreateMethod with { Statements = factoryMethodStatements }));

        return factoryNode.FactoryImplementation with
        {
            Fields = fields,
            Constructor = factoryNode.FactoryImplementation.Constructor with
            {
                Statements = factoryConstructorStatements,
            },
            CreateMethods = factoryMethods,
            DisposeForMethodImplementations = disposeMethodImplementations,
            DisposeMethodStatements = disposeMethodStatements,
        };
    }
}