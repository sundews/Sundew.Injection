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
using Sundew.Base;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Linq;
using Sundew.Base.Text;
using Sundew.Injection.Generator.TypeSystem;

internal class BindFactoryVisitor(
    GenericNameSyntax bindFactoryGenericNameSyntax,
    IMethodSymbol bindFactoryMethodSymbol,
    AnalysisContext analysisContext)
    : CSharpSyntaxWalker
{
    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        var factoryTypeSymbol = bindFactoryMethodSymbol.MapTypeArguments(bindFactoryGenericNameSyntax).Single();
        var parameters = bindFactoryMethodSymbol.Parameters.ByCardinality();
        switch (parameters)
        {
            case Empty<IParameterSymbol>:
                var factoryMethodResults = factoryTypeSymbol.TypeSymbol.GetMembers()
                    .Where(x => x.GetAttributes().FirstOrDefault(x =>
                        x.AttributeClass?.ToDisplayString() == KnownTypesProvider.BindableFactoryTargetName) != null)
                    .Select(symbol =>
                        symbol switch
                        {
                            IPropertySymbol propertySymbol =>
                                R.Success(analysisContext.TypeFactory.GetFactoryMethod(propertySymbol)
                                        .With(x => (Method: x, ReturnType: factoryTypeSymbol with { TypeSymbol = propertySymbol.Type })))
                                    .Omits<SymbolError>()
                                    .ToResultOption(),
                            IMethodSymbol methodSymbol =>
                                analysisContext.TypeFactory.GetFactoryMethod(methodSymbol)
                                    .With(x => (Method: x, ReturnType: factoryTypeSymbol with { TypeSymbol = methodSymbol.ReturnType }))
                                    .ToResultOption(),
                            _ => R.Error(new SymbolError(new NamedSymbol(symbol.ToDisplayString()), []))
                                .Omits<(Method Method, TypeSymbolWithLocation ReturnType)>()
                                .ToResultOption(),
                        })
                    .WhereNotDefault()
                    .AllOrFailed(x => x.ToItem());

                if (!factoryMethodResults.TryGet(out var all, out var factoryMethodErrors))
                {
                    factoryMethodErrors.ForEach(x => analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.InfiniteRecursionError, factoryTypeSymbol, x.Error.GetErrorText()));
                    return;
                }

                var factoryTypeResult = analysisContext.TypeFactory.GetFullType(factoryTypeSymbol);
                if (!factoryTypeResult.TryGet(out var factoryType, out var error))
                {
                    analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.InfiniteRecursionError, error);
                    return;
                }

                analysisContext.BindFactory(factoryType, all.Items);

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