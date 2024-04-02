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
using Sundew.Base.Collections.Linq;
using Sundew.Base.Text;
using Sundew.Injection.Generator.TypeSystem;

internal class BindFactoryVisitor(
    IMethodSymbol methodSymbol,
    AnalysisContext analysisContext)
    : CSharpSyntaxWalker
{
    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        var factoryTypeSymbol = methodSymbol.TypeArguments.Single();
        var parameters = methodSymbol.Parameters.ByCardinality();
        switch (parameters)
        {
            case Empty<IParameterSymbol>:
                var createMethods = factoryTypeSymbol.GetMembers().OfType<IMethodSymbol>()
                    .Where(x => x.GetAttributes().FirstOrDefault(x =>
                        x.AttributeClass?.ToDisplayString() == KnownTypesProvider.BindableCreateMethodName) != null).Select(x =>
                        (Method: analysisContext.TypeFactory.CreateMethod(x), ReturnType: x.ReturnType));
                analysisContext.BindFactory(analysisContext.TypeFactory.CreateType(factoryTypeSymbol), createMethods);
                break;
            case Single<IParameterSymbol>:
                new BindMethodVisitor(factoryTypeSymbol, analysisContext).Visit(node);
                break;
            case Multiple<IParameterSymbol> multiple:
                const string separator = ", ";
                analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostic.Create(Diagnostics.MultipleParametersNotSupportedForBindFactoryError, node.GetLocation(), DiagnosticSeverity.Error, multiple.Items.Select(x => x.Name).JoinToString(separator)));
                break;
        }
    }
}