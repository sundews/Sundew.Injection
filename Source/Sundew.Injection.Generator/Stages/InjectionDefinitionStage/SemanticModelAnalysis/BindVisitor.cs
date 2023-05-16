// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Injection.Generator.TypeSystem;

internal class BindVisitor : CSharpSyntaxWalker
{
    private readonly SemanticModel semanticModel;
    private readonly TypeFactory typeFactory;
    private readonly CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder;
    private readonly IMethodSymbol methodSymbol;

    public BindVisitor(SemanticModel semanticModel, TypeFactory typeFactory, CompiletimeInjectionDefinitionBuilder compiletimeInjectionDefinitionBuilder, IMethodSymbol methodSymbol)
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
        var constructorSelector = (Method?)parameters[i++].ExplicitDefaultValue;
        var isInjectable = (bool?)parameters[i++].ExplicitDefaultValue ?? false;
        var isNewOverridable = (bool?)parameters[i++].ExplicitDefaultValue ?? false;
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
                    case nameof(constructorSelector):
                        constructorSelector = this.GetMethod(argumentSyntax);
                        break;
                    case nameof(isInjectable):
                        isInjectable = GetBooleanLiteralValue(argumentSyntax);
                        break;
                    case nameof(isNewOverridable):
                        isNewOverridable = GetBooleanLiteralValue(argumentSyntax);
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
                        constructorSelector = this.GetMethod(argumentSyntax);
                        break;
                    case 2:
                        isInjectable = GetBooleanLiteralValue(argumentSyntax);
                        break;
                    case 3:
                        isNewOverridable = GetBooleanLiteralValue(argumentSyntax);
                        break;
                }

                argumentIndex++;
            }
        }

        var interfaceTypes = typeArguments.Take(typeArguments.Length - 1).Select(this.typeFactory.CreateType).ToImmutableArray();
        var implementationType = this.typeFactory.CreateType(typeArguments.Last());

        var actualMethod = constructorSelector;
        if (actualMethod == default)
        {
            if (implementationType.TypeMetadata.DefaultConstructor == null)
            {
                // Diagnostic
                return;
            }

            actualMethod = new Method(implementationType.TypeMetadata.DefaultConstructor.Parameters, implementationType.Type.Name, implementationType.Type, true);
        }

        this.compiletimeInjectionDefinitionBuilder.Bind(interfaceTypes, implementationType, actualMethod, scope, isInjectable, isNewOverridable);
    }

    private static bool GetBooleanLiteralValue(ArgumentSyntax argumentSyntax)
    {
        return argumentSyntax.Expression is LiteralExpressionSyntax literalExpressionSyntax && ((bool?)literalExpressionSyntax.Token.Value ?? false);
    }

    private Scope GetScope(ArgumentSyntax argumentSyntax)
    {
        return ExpressionAnalysisHelper.GetScope(this.semanticModel, argumentSyntax, this.typeFactory);
    }

    private Method? GetMethod(ArgumentSyntax argumentSyntax)
    {
        return ExpressionAnalysisHelper.GetMethod(argumentSyntax, this.semanticModel, this.typeFactory);
    }
}