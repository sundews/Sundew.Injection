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
using Sundew.Base;
using Sundew.Injection.Generator.TypeSystem;
using MethodKind = Microsoft.CodeAnalysis.MethodKind;

internal class AddFactoryMethodBindingVisitor(
    IMethodSymbol addMethodSymbol,
    AnalysisContext analysisContext)
    : CSharpSyntaxWalker
{
    private readonly List<(FactoryMethodTarget FactoryMethodTarget, TypeSymbolWithLocation ReturnType)> factoryMethodTargets = new();

    public IEnumerable<(FactoryMethodTarget FactoryMethodTarget, TypeSymbolWithLocation ReturnType)> FactoryMethodTargets => this.factoryMethodTargets;

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        this.VisitArgumentList(node.ArgumentList);
    }

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        base.VisitArgumentList(node);
        var parameters = addMethodSymbol.Parameters;
        var i = 0;
        var factoryMethodSelector = R.Success((Method: (Method?)parameters[i++].ExplicitDefaultValue, Location: Location.None)).Omits<ErrorWithLocation>();
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

        if (factoryMethodSelector.TryGetError(out var error, out var factoryMethod))
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(error);
            return;
        }

        if (factoryMethod.Method != default)
        {
            this.factoryMethodTargets.Add((new FactoryMethodTarget(factoryMethod.Method, analysisContext.TypeFactory.GetType(addMethodSymbol.ReturnType), false, addMethodSymbol.MethodKind == MethodKind.PropertyGet), new TypeSymbolWithLocation(addMethodSymbol.ReturnType, factoryMethodSelector.Value.Location)));
        }
    }

    private R<(Method? Method, Location Location), ErrorWithLocation> GetMethod(ArgumentSyntax argumentSyntax)
    {
        return ExpressionAnalysisHelper.GetMethod(argumentSyntax, analysisContext.SemanticModel, analysisContext.TypeFactory)
            .Map(method => (Method: method, argumentSyntax.GetLocation()));
    }
}