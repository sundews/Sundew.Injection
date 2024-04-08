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

internal class AddCreateMethodVisitor(
    IMethodSymbol methodSymbol,
    AnalysisContext analysisContext)
    : CSharpSyntaxWalker
{
    private readonly List<(Method Method, ITypeSymbol ReturnType)> createMethods = new();

    public IEnumerable<(Method Method, ITypeSymbol ReturnType)> CreateMethods => this.createMethods;

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        this.VisitArgumentList(node.ArgumentList);
    }

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        base.VisitArgumentList(node);
        var parameters = methodSymbol.Parameters;
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
            this.createMethods.Add(((Method Method, ITypeSymbol ReturnType))(factoryMethodSelector, methodSymbol.ReturnType));
        }
    }

    private Method? GetMethod(ArgumentSyntax argumentSyntax)
    {
        return ExpressionAnalysisHelper.GetMethod(argumentSyntax, analysisContext.SemanticModel, analysisContext.TypeFactory);
    }
}