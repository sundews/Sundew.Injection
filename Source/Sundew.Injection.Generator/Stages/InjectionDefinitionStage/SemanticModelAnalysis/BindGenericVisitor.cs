// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindGenericVisitor.cs" company="Sundews">
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
using Sundew.Base.Primitives;
using Sundew.Injection.Generator.TypeSystem;

internal class BindGenericVisitor : CSharpSyntaxWalker
{
    private readonly SemanticModel semanticModel;
    private readonly TypeFactory typeFactory;
    private readonly CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder;
    private readonly IMethodSymbol methodSymbol;

    public BindGenericVisitor(SemanticModel semanticModel, TypeFactory typeFactory, CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder, IMethodSymbol methodSymbol)
    {
        this.semanticModel = semanticModel;
        this.typeFactory = typeFactory;
        this.compiletimeInjectionDefinitionBuilder = compiletimeInjectionDefinitionBuilder;
        this.methodSymbol = methodSymbol;
    }

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        var typeArguments = this.methodSymbol.TypeArguments;
        var parameters = this.methodSymbol.Parameters;
        var i = 0;
        var scope = (Scope?)parameters[i++].ExplicitDefaultValue;
        var method = (GenericMethod?)parameters[i++].ExplicitDefaultValue;
        var argumentIndex = 0;
        foreach (var argumentSyntax in node.Arguments)
        {
            if (argumentSyntax.NameColon != null)
            {
                switch (argumentSyntax.NameColon.Name.ToString())
                {
                    case nameof(scope):
                        scope = this.GetScope(argumentSyntax);
                        break;
                    case nameof(method):
                        method = this.GetGenericMethod(argumentSyntax);
                        break;
                }
            }
            else
            {
                switch (argumentIndex)
                {
                    case 0:
                        scope = this.GetScope(argumentSyntax);
                        break;
                    case 1:
                        method = this.GetGenericMethod(argumentSyntax);
                        break;
                }

                argumentIndex++;
            }
        }

        var interfaces = typeArguments.Take(typeArguments.Length - 1).Select(x =>
        {
            if (x is INamedTypeSymbol namedTypeSymbol)
            {
                return this.typeFactory.GetUnboundGenericType(namedTypeSymbol.ConstructedFrom);
            }

            throw new NotSupportedException("Must be generic");
        }).ToImmutableArray();

        var last = typeArguments.Last();
        if (last is not INamedTypeSymbol lastNamedTypeSymbol)
        {
            throw new NotSupportedException("Must be generic");
        }

        var implementation = this.typeFactory.GetGenericType(lastNamedTypeSymbol);
        GenericMethod actualMethod = method.GetValueOrDefault();
        if (actualMethod == default)
        {
            actualMethod = lastNamedTypeSymbol.IsInstantiable() ? this.typeFactory.GetGenericMethod(lastNamedTypeSymbol.Constructors.GetMethodWithMostParameters()) : default;
            if (actualMethod == default)
            {
                // Diagnostic no constructor to bind to!
            }
        }

        this.compiletimeInjectionDefinitionBuilder.BindGeneric(interfaces, implementation, scope ?? Scope.Auto, actualMethod);
    }

    private Scope GetScope(ArgumentSyntax argumentSyntax)
    {
        return ExpressionAnalysisHelper.GetScope(this.semanticModel, argumentSyntax, this.typeFactory);
    }

    private GenericMethod? GetGenericMethod(ArgumentSyntax argumentSyntax)
    {
        return ExpressionAnalysisHelper.GetGenericMethod(argumentSyntax, this.semanticModel, this.typeFactory);
    }
}