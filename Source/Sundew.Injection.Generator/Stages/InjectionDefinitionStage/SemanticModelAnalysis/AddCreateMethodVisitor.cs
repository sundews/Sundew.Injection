// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddFactoryMethodVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Injection.Generator.TypeSystem;

internal class AddCreateMethodVisitor : CSharpSyntaxWalker
{
    private readonly IMethodSymbol methodSymbol;
    private readonly AnalysisContext analysisContext;
    private readonly List<(Method Method, ITypeSymbol ReturnType)> createMethods = new();

    public AddCreateMethodVisitor(IMethodSymbol methodSymbol, AnalysisContext analysisContext)
    {
        this.methodSymbol = methodSymbol;
        this.analysisContext = analysisContext;
    }

    public IEnumerable<(Method Method, ITypeSymbol ReturnType)> CreateMethods => this.createMethods;

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        this.VisitArgumentList(node.ArgumentList);
    }

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        base.VisitArgumentList(node);
        var typeArguments = this.methodSymbol.TypeArguments;
        var interfaceType = typeArguments[0];
        var implementationType = typeArguments.Length == 2 ? typeArguments[1] : interfaceType;
        var parameters = this.methodSymbol.Parameters;
        var i = 0;
        var factoryMethodSelector = (Method?)parameters[i++].ExplicitDefaultValue;
        var argumentIndex = 0;
        foreach (var argumentSyntax in node.Arguments)
        {
            if (argumentSyntax.NameColon != null)
            {
                switch (argumentSyntax.NameColon.Name.ToString())
                {
                    case nameof(factoryMethodSelector):
                        factoryMethodSelector = this.GetMethod(argumentSyntax);
                        break;
                }
            }
            else
            {
                switch (argumentIndex)
                {
                    case 0:
                        factoryMethodSelector = this.GetMethod(argumentSyntax);
                        break;
                }

                argumentIndex++;
            }
        }

        if (factoryMethodSelector != null)
        {
            this.createMethods.Add((factoryMethodSelector, this.methodSymbol.ReturnType));
        }
    }

    private Method? GetMethod(ArgumentSyntax argumentSyntax)
    {
        return ExpressionAnalysisHelper.GetMethod(argumentSyntax, this.analysisContext.SemanticModel, this.analysisContext.TypeFactory);
    }
}