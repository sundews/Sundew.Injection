// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindGenericVisitor.cs" company="Sundews">
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
using Sundew.Base.Collections;
using Sundew.Base.Collections.Linq;
using Sundew.Injection.Generator.Extensions;
using Sundew.Injection.Generator.TypeSystem;

internal class BindGenericVisitor(
    GenericNameSyntax genericNameSyntax,
    IMethodSymbol methodSymbol,
    AnalysisContext analysisContext)
    : CSharpSyntaxWalker
{
    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        var typeArguments = methodSymbol.MapTypeArguments(genericNameSyntax);

        var interfaces = typeArguments.Take(typeArguments.Length - 1).AllOrFailed(mappedTypeSymbol =>
        {
            if (mappedTypeSymbol.TypeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType)
            {
                return Item.Pass(analysisContext.TypeFactory.GetUnboundGenericType(namedTypeSymbol.ConstructedFrom)).Omits<Diagnostics>();
            }

            return Item.Fail(Diagnostics.Create(Diagnostics.OnlyGenericTypeSupportedError, mappedTypeSymbol));
        });

        var actualInterfaces = interfaces.TryGet(out var all, out var failed)
            ? all.Items.ToImmutableArray()
            : failed.Items.Do(
                errors => errors.ForEach(x => analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostics(x.Error)),
                ImmutableArray<(UnboundGenericType Type, TypeMetadata TypeMetadata)>.Empty);

        var last = typeArguments.Last();
        if (last.TypeSymbol is not INamedTypeSymbol lastNamedTypeSymbol || !lastNamedTypeSymbol.IsGenericType)
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.OnlyGenericTypeSupportedError, last);
            return;
        }

        var implementation = analysisContext.TypeFactory.GetGenericType(lastNamedTypeSymbol);
        var parameters = methodSymbol.Parameters;
        var i = 0;
        var scope = new ScopeContext((Scope?)parameters[i++].ExplicitDefaultValue ?? Scope._Auto, ScopeSelection.Implicit);
        var method = R.SuccessOption((GenericMethod?)parameters[i++].ExplicitDefaultValue).Omits<SymbolErrorWithLocation>();
        var argumentIndex = 0;
        foreach (var argumentSyntax in node.Arguments)
        {
            if (argumentSyntax.NameColon != null)
            {
                switch (argumentSyntax.NameColon.Name.ToString())
                {
                    case nameof(scope):
                        scope = this.GetScope(argumentSyntax, implementation.Type);
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
                        scope = this.GetScope(argumentSyntax, implementation.Type);
                        break;
                    case 1:
                        method = this.GetGenericMethod(argumentSyntax);
                        break;
                }

                argumentIndex++;
            }
        }

        if (method.IsError)
        {
            analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.InfiniteRecursionError, method.Error);
            return;
        }

        var actualMethod = method.Value.GetValueOrDefault();
        if (actualMethod == default)
        {
            if (!lastNamedTypeSymbol.IsInstantiable())
            {
                analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.TypeNotInstantiableError, last);
                return;
            }

            var genericMethodResult = analysisContext.TypeFactory.GetGenericMethod(lastNamedTypeSymbol.Constructors.GetDefaultMethodWithMostParameters());
            if (genericMethodResult.IsError)
            {
                analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.InfiniteRecursionError, last, genericMethodResult.Error.GetErrorText());
                return;
            }

            actualMethod = genericMethodResult.Value;
            if (actualMethod == default)
            {
                analysisContext.CompiletimeInjectionDefinitionBuilder.AddDiagnostic(Diagnostics.NoViableConstructorFoundError, last);
                return;
            }
        }

        analysisContext.CompiletimeInjectionDefinitionBuilder.BindGeneric(actualInterfaces, implementation, scope, actualMethod);
    }

    private ScopeContext GetScope(ArgumentSyntax argumentSyntax, Symbol targetType)
    {
        return ExpressionAnalysisHelper.GetScope(analysisContext.SemanticModel, argumentSyntax, analysisContext.TypeFactory, targetType);
    }

    private R<GenericMethod?, SymbolErrorWithLocation> GetGenericMethod(ArgumentSyntax argumentSyntax)
    {
        return ExpressionAnalysisHelper.GetGenericMethod(argumentSyntax, analysisContext.SemanticModel, analysisContext.TypeFactory).ToValueOptionResult();
    }
}