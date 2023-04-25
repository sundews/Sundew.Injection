// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownSyntax.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Syntax;

using System;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.CompilationDataStage;
using Sundew.Injection.Generator.Stages.FactoryDataStage;

internal class KnownSyntax
{
    private const string This = "this";
    private readonly Lazy<DisposingListSyntax> factoryConstructorDisposingList;
    private readonly Lazy<WeakKeyDisposingDictionarySyntax> factoryMethodWeakKeyDisposingDictionary;
    private readonly Lazy<ConstructorCall> disposableListConstructorCall;
    private readonly Lazy<DisposingListSyntax> localDisposableList;

    public KnownSyntax(CompilationData compilationData)
    {
        this.factoryConstructorDisposingList = new Lazy<DisposingListSyntax>(() =>
        {
            var memberAccessExpression = new MemberAccessExpression(
                new Identifier(This),
                FactorySyntaxGenerator.FactoryConstructorDisposingListName);
            return new DisposingListSyntax(memberAccessExpression);
        });
        this.factoryMethodWeakKeyDisposingDictionary = new Lazy<WeakKeyDisposingDictionarySyntax>(() =>
        {
            var memberAccessExpression = new MemberAccessExpression(
                new Identifier(This),
                FactorySyntaxGenerator.FactoryMethodDisposingDictionaryName);
            return new WeakKeyDisposingDictionarySyntax(memberAccessExpression);
        });
        this.localDisposableList = new Lazy<DisposingListSyntax>(() => new DisposingListSyntax(new Identifier(FactorySyntaxGenerator.LocalDisposingListName)));
        this.disposableListConstructorCall = new(() => new ConstructorCall(compilationData.DisposableListType));
    }

    public ConstructorCall DisposerConstructorCall => this.disposableListConstructorCall.Value;

    public DisposingListSyntax FactoryConstructorDisposingListSyntax => this.factoryConstructorDisposingList.Value;

    public DisposingListSyntax LocalDisposingListSyntax => this.localDisposableList.Value;

    public WeakKeyDisposingDictionarySyntax FactoryMethodWeakKeyDisposingDictionarySyntax => this.factoryMethodWeakKeyDisposingDictionary.Value;

    public sealed record DisposingListSyntax(Expression DisposableListAccess, MemberAccessExpression AddMethod, MemberAccessExpression DisposeMethod)
    {
        internal const string DisposeMethodName = "Dispose";
        internal const string AddName = "Add";

        public DisposingListSyntax(Expression disposableListField)
            : this(
                disposableListField,
                new MemberAccessExpression(disposableListField, AddName),
                new MemberAccessExpression(disposableListField, DisposeMethodName))
        {
        }
    }

    public sealed record WeakKeyDisposingDictionarySyntax(Expression WeakKeyDisposingDictionaryAccess, MemberAccessExpression TryAddMethod, MemberAccessExpression DisposeMethod)
    {
        internal const string DisposeMethodName = "Dispose";
        internal const string TryAddName = "TryAdd";

        public WeakKeyDisposingDictionarySyntax(MemberAccessExpression disposableListField)
            : this(
                disposableListField,
                new MemberAccessExpression(disposableListField, TryAddName),
                new MemberAccessExpression(disposableListField, DisposeMethodName))
        {
        }
    }
}