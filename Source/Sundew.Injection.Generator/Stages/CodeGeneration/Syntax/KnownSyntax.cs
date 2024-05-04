// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownSyntax.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

using System;
using System.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.TypeSystem;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

internal class KnownSyntax
{
    internal const string LifecycleHandlerName = "lifecycleHandler";
    internal const string ChildLifecycleHandlerName = "childLifecycleHandler";
    private const string CreateChildLifecycleHandler = "CreateChildLifecycleHandler";
    private const string TryAdd = "TryAdd";
    private const string InitializeAsync = "InitializeAsync";
    private const string Dispose = "Dispose";
    private const string DisposeAsync = "DisposeAsync";
    private readonly Lazy<LifecycleHandlerSyntax> lifecycleHandler;
    private readonly Lazy<LifecycleHandlerSyntax> childLifecycleHandler;

    public KnownSyntax(CompilationData compilationData)
    {
        this.lifecycleHandler = new Lazy<LifecycleHandlerSyntax>(() =>
        {
            var memberAccessExpression = new MemberAccessExpression(
                Identifier.This,
                LifecycleHandlerName);
            return new LifecycleHandlerSyntax(compilationData.ProvidedSundewInjectionCompilationData.LifecycleHandlerBinding.TargetType, compilationData.ReferencedSundewInjectionCompilationData.ILifecycleHandlerType, memberAccessExpression, LifecycleHandlerName);
        });

        this.childLifecycleHandler = new Lazy<LifecycleHandlerSyntax>(() =>
        {
            var memberAccessExpression = new Identifier(ChildLifecycleHandlerName);
            return new LifecycleHandlerSyntax(compilationData.ProvidedSundewInjectionCompilationData.LifecycleHandlerBinding.ReferencedType, compilationData.ReferencedSundewInjectionCompilationData.ILifecycleHandlerType, memberAccessExpression, ChildLifecycleHandlerName);
        });
    }

    public LifecycleHandlerSyntax SharedLifecycleHandler => this.lifecycleHandler.Value;

    public LifecycleHandlerSyntax ChildLifecycleHandler => this.childLifecycleHandler.Value;

    public string DisposeName => Dispose;

    public string DisposeAsyncName => DisposeAsync;

    public string InitializeAsyncName => InitializeAsync;

    public AttributeDeclaration EditorBrowsableAttribute { get; } = new(
        "[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]");

    public AttributeDeclaration FactoryAttribute { get; } = new(
        "[global::Sundew.Injection.Factory]");

    public AttributeDeclaration BindableCreateMethodAttribute { get; } = new(
        $"[global::{KnownTypesProvider.BindableFactoryTargetName}]");

    public AttributeDeclaration IndirectCreateMethodAttribute { get; } = new(
        $"[global::{KnownTypesProvider.IndirectFactoryTargetName}]");

    public sealed record LifecycleHandlerSyntax(
        string AccessorName,
        Type Type,
        Expression Access,
        ExpressionStatement CreateLifecycleHandlerAndAssignFieldStatement,
        LocalDeclarationStatement CreateChildLifecycleHandlerAndAssignVarStatement,
        MemberAccessExpression TryAddMethod,
        MemberAccessExpression InitializeMethod,
        AwaitExpression InitializeAsyncMethodCall,
        MemberAccessExpression DisposeMethod,
        MemberAccessExpression DisposeAsyncMethod,
        ParameterDeclaration OnCreateMethodParameterDeclaration)
    {
        private const string Initialize = "Initialize";
        private const string ConfigureAwait = "ConfigureAwait";

        public LifecycleHandlerSyntax(Type targetLifetimeHandlerType, Type referencedLifetimeHandlerType, Expression lifetimeHandlerAccess, string accessorName)
            : this(
                accessorName,
                targetLifetimeHandlerType,
                lifetimeHandlerAccess,
                new ExpressionStatement(new AssignmentExpression(lifetimeHandlerAccess, CreationExpression._ConstructorCall(targetLifetimeHandlerType, ImmutableList.Create(Literal.False, Literal.False, Literal.Null, Literal.Null)))),
                new LocalDeclarationStatement(ChildLifecycleHandlerName, new InvocationExpression(new MemberAccessExpression(lifetimeHandlerAccess, CreateChildLifecycleHandler))),
                new MemberAccessExpression(lifetimeHandlerAccess, TryAdd),
                new MemberAccessExpression(lifetimeHandlerAccess, Initialize),
                new AwaitExpression(new InvocationExpression(new MemberAccessExpression(new InvocationExpression(new MemberAccessExpression(lifetimeHandlerAccess, InitializeAsync)), ConfigureAwait), [Literal.False])),
                new MemberAccessExpression(lifetimeHandlerAccess, Dispose),
                new MemberAccessExpression(lifetimeHandlerAccess, DisposeAsync),
                new ParameterDeclaration(referencedLifetimeHandlerType, "lifecycleHandler"))
        {
        }
    }
}