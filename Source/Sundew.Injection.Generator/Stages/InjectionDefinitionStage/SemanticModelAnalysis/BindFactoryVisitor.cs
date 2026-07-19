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
        var parameters = node.Arguments.ByCardinality();
        switch (parameters)
        {
            case Empty<ArgumentSyntax>:
                var factoryMethodResults = factoryTypeSymbol.TypeSymbol.GetMembers()
                    .Where(x => x.GetAttributes().FirstOrDefault(x =>
                        x.AttributeClass?.ToDisplayString() == KnownTypesProvider.BindableFactoryTargetName) != null)
                    .Select(symbol =>
                        symbol switch
                        {
                            IPropertySymbol propertySymbol => analysisContext.TypeFactory.GetFactoryMethodTarget(propertySymbol)
                                        .Map(x => (Method: x, ReturnType: factoryTypeSymbol with { TypeSymbol = propertySymbol.Type })),
                            IMethodSymbol methodSymbol =>
                                analysisContext.TypeFactory.GetFactoryMethodTarget(methodSymbol)
                                    .Map(x => (Method: x, ReturnType: factoryTypeSymbol with { TypeSymbol = methodSymbol.ReturnType })),
                            _ => R.Error(new Error(ErrorType.UnsupportedSymbol, new NamedSymbol(symbol.ToDisplayString()), []))
                                .Omits<(FactoryMethodTarget Method, TypeSymbolWithLocation ReturnType)>(),
                        })
                    .AllOrFailed(x => x.ToItem());

                if (factoryMethodResults.TryGetError(out var factoryMethodErrors, out var all))
                {
                    factoryMethodErrors.ForEach(x => analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(new ErrorWithLocation(x.Error, node.GetLocation())));
                    return;
                }

                var factoryTypeResult = analysisContext.TypeFactory.GetFullType(factoryTypeSymbol);
                if (factoryTypeResult.TryGetError(out var errorWithLocation, out var factoryType))
                {
                    analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(errorWithLocation);
                    return;
                }

                analysisContext.BindFactory(factoryType, all.Items);

                break;
            case Single<ArgumentSyntax>:
                new BindFactoryMethodVisitor(factoryTypeSymbol, analysisContext).Visit(node);
                break;
            case Multiple<ArgumentSyntax>:
                const string separator = ", ";
                analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostic.Create(Diagnostics.MultipleParametersNotSupportedForBindFactoryError, node.GetLocation(), bindFactoryMethodSymbol.Parameters.Select(x => x.Name).JoinToString(separator)));
                break;
        }
    }
}