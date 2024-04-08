// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Diagnostics.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

public sealed record Diagnostics(ValueList<Diagnostic> Items) : IEnumerable<Diagnostic>
{
    private const string CodeGeneration = "CodeGeneration";

    public Diagnostics(params Diagnostic[] diagnostics)
        : this(diagnostics.ToValueList())
    {
    }

    public Diagnostics(IEnumerable<Diagnostic> diagnostics)
        : this(diagnostics.ToValueList())
    {
    }

    public static DiagnosticDescriptor UnknownError { get; } = new(
        "SI0100",
        Resources.UnknownErrorTitle,
        Resources.UnknownErrorMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.UnknownErrorDescription);

    public static DiagnosticDescriptor InvalidFactoryTypeError { get; } = new(
        "SI0001",
        Resources.InvalidFactoryTypeTitle,
        Resources.InvalidFactoryTypeMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.InvalidFactoryTypeDescription);

    public static DiagnosticDescriptor ScopeError { get; } = new(
        "SI0002",
        Resources.ScopeErrorTitle,
        Resources.ScopeErrorMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.ScopeErrorDescription);

    public static DiagnosticDescriptor ResolveRequiredParameterError { get; } = new(
        "SI0003",
        Resources.ResolveRequiredParameterErrorTitle,
        Resources.ResolveRequiredParameterErrorMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.ResolveRequiredParameterErrorDescription);

    public static DiagnosticDescriptor CreateGenericMethodError { get; } = new(
        "SI0004",
        Resources.CreateGenericMethodErrorTitle,
        Resources.CreateGenericMethodErrorMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.CreateGenericMethodErrorDescription);

    public static DiagnosticDescriptor UnsupportedInstanceMethodError { get; } = new(
        "SI0005",
        Resources.UnsupportedInstanceMethodErrorTitle,
        Resources.UnsupportedInstanceMethodErrorMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.UnsupportedInstanceMethodErrorDescription);

    public static DiagnosticDescriptor OnlyGenericTypeSupportedError { get; } = new(
        "SI0006",
        Resources.OnlyGenericTypeSupportedErrorTitle,
        Resources.OnlyGenericTypeSupportedErrorMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.OnlyGenericTypeSupportedErrorDescription);

    public static DiagnosticDescriptor TypeNotInstantiableError { get; } = new(
        "SI0007",
        Resources.TypeNotInstantiableErrorTitle,
        Resources.TypeNotInstantiableErrorMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.TypeNotInstantiableErrorDescription);

    public static DiagnosticDescriptor NoViableConstructorFoundError { get; } = new(
        "SI0008",
        Resources.NoViableConstructorFoundErrorTitle,
        Resources.NoViableConstructorFoundErrorMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.NoViableConstructorFoundErrorDescription);

    public static DiagnosticDescriptor RequiredTypeNotFoundError { get; } = new(
        "SI0009",
        Resources.RequiredTypeNotFoundErrorTitle,
        Resources.RequiredTypeNotFoundErrorMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.RequiredTypeNotFoundErrorDescription);

    public static DiagnosticDescriptor MultipleParametersNotSupportedForBindFactoryError { get; } = new(
        "SI0009",
        Resources.MultipleParametersNotSupportedForBindFactoryErrorTitle,
        Resources.MultipleParametersNotSupportedForBindFactoryErrorMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.MultipleParametersNotSupportedForBindFactoryErrorDescription);

    public static DiagnosticDescriptor NoFactoryMethodFoundForTypeError { get; } = new(
        "SI0010",
        Resources.NoFactoryMethodFoundForTypeErrorTitle,
        Resources.NoFactoryMethodFoundForTypeErrorMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.NoFactoryMethodFoundForTypeErrorDescription);

    public static DiagnosticDescriptor NoBindingFoundForNonConstructableTypeError { get; } = new(
        "SI0011",
        Resources.NoBindingFoundForNonConstructableTypeErrorTitle,
        Resources.NoBindingFoundForNonConstructableTypeErrorMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.NoBindingFoundForNonConstructableTypeErrorDescription);

    public static Diagnostics Create(DiagnosticDescriptor diagnosticDescriptor, MappedTypeSymbol mappedTypeSymbol)
    {
        return Create(diagnosticDescriptor, mappedTypeSymbol.TypeSymbol, mappedTypeSymbol.OriginatingSyntaxNode);
    }

    public static Diagnostics Create(DiagnosticDescriptor diagnosticDescriptor, ISymbol symbol, SyntaxNode? originatingSyntaxNode = default)
    {
        if (originatingSyntaxNode.HasValue())
        {
            return new Diagnostics(Diagnostic.Create(diagnosticDescriptor, originatingSyntaxNode.GetLocation(), symbol.ToDisplayString()));
        }

        if (symbol.DeclaringSyntaxReferences.IsEmpty)
        {
            return new Diagnostics(Diagnostic.Create(diagnosticDescriptor, Location.None, symbol.ToDisplayString()));
        }

        return new Diagnostics(symbol.DeclaringSyntaxReferences.Select(x =>
            Diagnostic.Create(diagnosticDescriptor, x.GetSyntax().GetLocation(), symbol.ToDisplayString())));
    }

    public IEnumerator<Diagnostic> GetEnumerator()
    {
        return this.Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}