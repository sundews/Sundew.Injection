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
using Sundew.Base.Collections;
using Sundew.Base.Collections.Linq;
using Sundew.Injection.Generator.Extensions;
using Sundew.Injection.Generator.TypeSystem;

internal class BindGenericVisitor : CSharpSyntaxWalker
{
    private readonly IMethodSymbol methodSymbol;
    private readonly AnalysisContext analysisContext;

    public BindGenericVisitor(IMethodSymbol methodSymbol, AnalysisContext analysisContext)
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
        var method = (GenericMethod?)parameters[i++].ExplicitDefaultValue;
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
                        scope = this.GetScope(argumentSyntax);
                        break;
                    case 1:
                        method = this.GetGenericMethod(argumentSyntax);
                        break;
                }

                argumentIndex++;
            }
        }

        var interfaces = typeArguments.Take(typeArguments.Length - 1).AllOrFailed(x =>
        {
            if (x is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType)
            {
                return Item.Pass(this.analysisContext.TypeFactory.GetUnboundGenericType(namedTypeSymbol.ConstructedFrom));
            }

            return Item.Fail<(UnboundGenericType Type, TypeMetadata TypeMetadata), Diagnostic>(Diagnostic.Create(Diagnostics.OnlyGenericTypeSupportedError, default, x.ToDisplayString()));
        });

        var actualInterfaces = interfaces.TryGet(out var all, out var failed)
            ? all.Items.ToImmutableArray()
            : failed.Items.Do(
                errors => errors.ForEach(x => this.analysisContext.CompiletimeInjectionDefinitionBuilder.ReportDiagnostic(x.Error)),
                ImmutableArray<(UnboundGenericType Type, TypeMetadata TypeMetadata)>.Empty);

        var last = typeArguments.Last();
        if (last is not INamedTypeSymbol lastNamedTypeSymbol || !lastNamedTypeSymbol.IsGenericType)
        {
            this.analysisContext.CompiletimeInjectionDefinitionBuilder.ReportDiagnostic(Diagnostic.Create(Diagnostics.OnlyGenericTypeSupportedError, default, last.ToDisplayString()));
            return;
        }

        var implementation = this.analysisContext.TypeFactory.GetGenericType(lastNamedTypeSymbol);
        GenericMethod actualMethod = method.GetValueOrDefault();
        if (actualMethod == default)
        {
            if (!lastNamedTypeSymbol.IsInstantiable())
            {
                this.analysisContext.CompiletimeInjectionDefinitionBuilder.ReportDiagnostic(Diagnostic.Create(Diagnostics.TypeNotInstantiableError, default, last.ToDisplayString()));
                return;
            }

            actualMethod = this.analysisContext.TypeFactory.GetGenericMethod(lastNamedTypeSymbol.Constructors.GetDefaultMethodWithMostParameters());
            if (actualMethod == default)
            {
                this.analysisContext.CompiletimeInjectionDefinitionBuilder.ReportDiagnostic(Diagnostic.Create(Diagnostics.NoViableConstructorFoundError, default, last.ToDisplayString()));
                return;
            }
        }

        this.analysisContext.CompiletimeInjectionDefinitionBuilder.BindGeneric(actualInterfaces, implementation, scope ?? Scope._Auto, actualMethod);
    }

    private Scope GetScope(ArgumentSyntax argumentSyntax)
    {
        return ExpressionAnalysisHelper.GetScope(this.analysisContext.SemanticModel, argumentSyntax, this.analysisContext.TypeFactory);
    }

    private GenericMethod? GetGenericMethod(ArgumentSyntax argumentSyntax)
    {
        return ExpressionAnalysisHelper.GetGenericMethod(argumentSyntax, this.analysisContext.SemanticModel, this.analysisContext.TypeFactory);
    }
}