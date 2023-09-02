// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;
using MethodKind = Sundew.Injection.Generator.TypeSystem.MethodKind;
using Type = Sundew.Injection.Generator.TypeSystem.Type;

internal class BindFactoryVisitor : CSharpSyntaxWalker
{
    private readonly SemanticModel semanticModel;
    private readonly TypeFactory typeFactory;
    private readonly KnownAnalysisTypes knownAnalysisTypes;
    private readonly CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder;
    private readonly IMethodSymbol methodSymbol;

    public BindFactoryVisitor(
        SemanticModel semanticModel,
        TypeFactory typeFactory,
        KnownAnalysisTypes knownAnalysisTypes,
        CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder,
        IMethodSymbol methodSymbol)
    {
        this.semanticModel = semanticModel;
        this.typeFactory = typeFactory;
        this.knownAnalysisTypes = knownAnalysisTypes;
        this.compiletimeInjectionDefinitionBuilder = compiletimeInjectionDefinitionBuilder;
        this.methodSymbol = methodSymbol;
    }

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        var factoryTypeSymbol = this.methodSymbol.TypeArguments.Single();
        var parameters = this.methodSymbol.Parameters.ByCardinality();
        switch (parameters)
        {
            case Empty<IParameterSymbol>:
                this.GetBindings(factoryTypeSymbol);
                break;
            case Single<IParameterSymbol> single:
                var isSingleRegistration = single.Item.Type.Name switch
                {
                    "Expression" => true,
                    "Func" => false,
                    _ => throw new NotSupportedException("Only Expression or Func are supported for BindFactory"),
                };
                break;
            case Multiple<IParameterSymbol>:
                throw new NotSupportedException("Multiple items are not supported for BindFactory");
        }
    }

    private void GetBindings(ITypeSymbol factoryTypeSymbol)
    {
        var createMethods = factoryTypeSymbol.GetMembers().OfType<IMethodSymbol>()
            .Where(x => x.GetAttributes().FirstOrDefault(x =>
                x.AttributeClass?.ToDisplayString() == typeof(CreateMethodAttribute).FullName) != null).Select(x =>
                (Method: this.typeFactory.CreateMethod(x), x.ReturnType));
        var factoryType = this.typeFactory.CreateType(factoryTypeSymbol);
        if (!this.compiletimeInjectionDefinitionBuilder.HasBinding(factoryType.Type) && factoryType.TypeMetadata.DefaultConstructor != null)
        {
            var actualMethod = new Method(factoryType.TypeMetadata.DefaultConstructor!.Parameters, factoryType.Type.Name, factoryType.Type, MethodKind._Constructor);
            this.compiletimeInjectionDefinitionBuilder.Bind(ImmutableArray<(Type Type, TypeMetadata TypeMetadata)>.Empty, factoryType, actualMethod, Scope.SingleInstancePerFactory, false, false);
        }

        foreach (var methodAndReturnType in createMethods)
        {
            var returnType = this.typeFactory.CreateType(methodAndReturnType.ReturnType);
            this.compiletimeInjectionDefinitionBuilder.Bind(ImmutableArray<(Type Type, TypeMetadata TypeMetadata)>.Empty, returnType, methodAndReturnType.Method, Scope.Auto, false, false);

            if (SymbolEqualityComparer.Default.Equals(methodAndReturnType.ReturnType.OriginalDefinition, this.knownAnalysisTypes.ConstructedTypeSymbol))
            {
                this.compiletimeInjectionDefinitionBuilder.Bind(ImmutableArray<(Type Type, TypeMetadata TypeMetadata)>.Empty, this.typeFactory.CreateType(((INamedTypeSymbol)methodAndReturnType.ReturnType).TypeArguments.Single()), new Method(ImmutableArray<Parameter>.Empty, nameof(Constructed<object>.Object), this.typeFactory.CreateType(methodAndReturnType.ReturnType).Type, MethodKind._Instance(returnType.TypeMetadata with { HasLifetime = false }, true)), Scope.Auto, false, false);
            }
        }
    }
}