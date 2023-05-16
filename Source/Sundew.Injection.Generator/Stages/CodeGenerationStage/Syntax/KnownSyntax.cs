// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownSyntax.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;

using System;
using System.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage;
using Sundew.Injection.Generator.TypeSystem;

internal class KnownSyntax
{
    internal const string LifetimeHandlerName = "lifecycleHandler";
    internal const string ChildLifetimeHandlerName = "childLifetimeHandler";
    private const string CreateChildLifetimeHandler = "CreateChildLifecycleHandler";
    private const string TryAdd = "TryAdd";
    private const string InitializeAsync = "InitializeAsync";
    private const string Dispose = "Dispose";
    private const string DisposeAsync = "DisposeAsync";
    private readonly Lazy<LifetimeHandlerSyntax> lifetimeHandler;
    private readonly Lazy<LifetimeHandlerSyntax> childLifetimeHandler;

    public KnownSyntax(CompilationData compilationData)
    {
        this.lifetimeHandler = new Lazy<LifetimeHandlerSyntax>(() =>
        {
            var memberAccessExpression = new MemberAccessExpression(
                Identifier.This,
                LifetimeHandlerName);
            return new LifetimeHandlerSyntax(compilationData.LifetimeHandlerType, compilationData.LifetimeHandlerType, memberAccessExpression, LifetimeHandlerName);
        });

        this.childLifetimeHandler = new Lazy<LifetimeHandlerSyntax>(() =>
        {
            var memberAccessExpression = new Identifier(ChildLifetimeHandlerName);
            return new LifetimeHandlerSyntax(compilationData.ILifetimeHandlerType, compilationData.LifetimeHandlerType, memberAccessExpression, ChildLifetimeHandlerName);
        });
    }

    public LifetimeHandlerSyntax SharedLifetimeHandler => this.lifetimeHandler.Value;

    public LifetimeHandlerSyntax ChildLifetimeHandler => this.childLifetimeHandler.Value;

    public string DisposeName => Dispose;

    public string DisposeAsyncName => DisposeAsync;

    public string InitializeAsyncName => InitializeAsync;

    public AttributeDeclaration EditorBrowsableAttribute { get; } = new AttributeDeclaration(
        "[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]");

    public sealed record LifetimeHandlerSyntax(
        string AccessorName,
        NamedType Type,
        Expression Access,
        ExpressionStatement CreateLifetimeHandlerAndAssignFieldStatement,
        LocalDeclarationStatement CreateChildLifetimeHandlerAndAssignVarStatement,
        MemberAccessExpression TryAddMethod,
        MemberAccessExpression InitializeMethod,
        AwaitExpression InitializeAsyncMethodCall,
        MemberAccessExpression DisposeMethod,
        MemberAccessExpression DisposeAsyncMethod)
    {
        private const string Initialize = "Initialize";
        private const string False = "false";
        private const string ConfigureAwait = "ConfigureAwait";

        public LifetimeHandlerSyntax(NamedType lifetimeHandlerType, NamedType createdLifetimeHandlerType, Expression lifetimeHandlerAccess, string accessorName)
            : this(
                accessorName,
                lifetimeHandlerType,
                lifetimeHandlerAccess,
                new ExpressionStatement(new AssignmentExpression(lifetimeHandlerAccess, new CreationExpression(CreationSource.ConstructorCall(createdLifetimeHandlerType), ImmutableList.Create(new Identifier(False), Identifier.Null, Identifier.Null)))),
                new LocalDeclarationStatement(ChildLifetimeHandlerName, new InvocationExpression(new MemberAccessExpression(lifetimeHandlerAccess, CreateChildLifetimeHandler))),
                new MemberAccessExpression(lifetimeHandlerAccess, TryAdd),
                new MemberAccessExpression(lifetimeHandlerAccess, Initialize),
                new AwaitExpression(new InvocationExpression(new MemberAccessExpression(new InvocationExpression(new MemberAccessExpression(lifetimeHandlerAccess, InitializeAsync)), ConfigureAwait), new Expression[] { new Identifier(False) })),
                new MemberAccessExpression(lifetimeHandlerAccess, Dispose),
                new MemberAccessExpression(lifetimeHandlerAccess, DisposeAsync))
        {
        }
    }
}