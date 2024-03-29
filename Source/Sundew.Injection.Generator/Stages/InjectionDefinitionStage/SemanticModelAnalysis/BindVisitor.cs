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
using Sundew.Base;
using Sundew.Injection.Generator.TypeSystem;

internal class BindVisitor : CSharpSyntaxWalker
{
    private readonly IMethodSymbol methodSymbol;
    private readonly AnalysisContext analysisContext;

    public BindVisitor(IMethodSymbol methodSymbol, AnalysisContext analysisContext)
    {
        this.methodSymbol = methodSymbol;
        this.analysisContext = analysisContext;
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
                        scope = this.GetScope(argumentSyntax).Scope;
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
                        scope = this.GetScope(argumentSyntax).Scope;
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

        var interfaceTypes = typeArguments.Take(typeArguments.Length - 1).Select(this.analysisContext.TypeFactory.CreateType).ToImmutableArray();
        var implementationType = this.analysisContext.TypeFactory.CreateType(typeArguments.Last());

        var actualMethodOption = constructorSelector ?? implementationType.TypeMetadata.DefaultConstructor;
        if (actualMethodOption.TryGetValue(out var actualMethod))
        {
            this.analysisContext.CompiletimeInjectionDefinitionBuilder.Bind(interfaceTypes, implementationType, actualMethod, scope, isInjectable, isNewOverridable);
            return;
        }

        this.analysisContext.CompiletimeInjectionDefinitionBuilder.ReportDiagnostic(Diagnostic.Create(Diagnostics.NoFactoryMethodFoundForTypeError, null, implementationType.Type));
    }

    private static bool GetBooleanLiteralValue(ArgumentSyntax argumentSyntax)
    {
        return argumentSyntax.Expression is LiteralExpressionSyntax literalExpressionSyntax && ((bool?)literalExpressionSyntax.Token.Value ?? false);
    }

    private (Scope Scope, ScopeOrigin Origin) GetScope(ArgumentSyntax argumentSyntax)
    {
        return ExpressionAnalysisHelper.GetScope(this.analysisContext.SemanticModel, argumentSyntax, this.analysisContext.TypeFactory);
    }

    private Method? GetMethod(ArgumentSyntax argumentSyntax)
    {
        return ExpressionAnalysisHelper.GetMethod(argumentSyntax, this.analysisContext.SemanticModel, this.analysisContext.TypeFactory);
    }
}