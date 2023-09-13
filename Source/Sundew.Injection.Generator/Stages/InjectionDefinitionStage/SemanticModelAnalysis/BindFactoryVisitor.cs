// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base.Collections;
using Sundew.Base.Text;

internal class BindFactoryVisitor : CSharpSyntaxWalker
{
    private readonly IMethodSymbol methodSymbol;
    private readonly AnalysisContext analysisContext;

    public BindFactoryVisitor(
        IMethodSymbol methodSymbol,
        AnalysisContext analysisContext)
    {
        this.methodSymbol = methodSymbol;
        this.analysisContext = analysisContext;
    }

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        var factoryTypeSymbol = this.methodSymbol.TypeArguments.Single();
        var parameters = this.methodSymbol.Parameters.ByCardinality();
        switch (parameters)
        {
            case Empty<IParameterSymbol>:
                var createMethods = factoryTypeSymbol.GetMembers().OfType<IMethodSymbol>()
                    .Where(x => x.GetAttributes().FirstOrDefault(x =>
                        x.AttributeClass?.ToDisplayString() == typeof(CreateMethodAttribute).FullName) != null).Select(x =>
                        (Method: this.analysisContext.TypeFactory.CreateMethod(x), ReturnType: x.ReturnType));
                this.analysisContext.BindFactory(this.analysisContext.TypeFactory.CreateType(factoryTypeSymbol), createMethods);
                break;
            case Single<IParameterSymbol>:
                new BindMethodVisitor(factoryTypeSymbol, this.analysisContext).Visit(node);
                break;
            case Multiple<IParameterSymbol> multiple:
                const string separator = ", ";
                this.analysisContext.CompiletimeInjectionDefinitionBuilder.ReportDiagnostic(Diagnostic.Create(Diagnostics.MultipleParametersNotSupportedForBindFactoryError, node.GetLocation(), DiagnosticSeverity.Error, multiple.Items.Select(x => x.Name).JoinToString(separator)));
                break;
        }
    }
}