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
using Sundew.Base.Collections;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;
using Sundew.Injection.Generator.TypeSystem;
using ISymbol = Microsoft.CodeAnalysis.ISymbol;

internal sealed record Diagnostics(ValueList<Diagnostic> Items) : IEnumerable<Diagnostic>
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

    public static DiagnosticDescriptor InfiniteRecursionError { get; } = new(
        "SI0012",
        Resources.InfiniteRecursionTitle,
        Resources.InfiniteRecursionMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.InfiniteRecursionDescription);

    public static DiagnosticDescriptor ReferencedTypeMismatchError { get; } = new(
        "SI0013",
        Resources.ReferencedTypeMismatchTitle,
        Resources.ReferencedTypeMismatchMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.ReferencedTypeMismatchDescription);

    public static Diagnostics Create(DiagnosticDescriptor diagnosticDescriptor, SymbolErrorWithLocation symbolErrorWithLocation, params object[] additionalArguments)
    {
        var arguments = new object[] { symbolErrorWithLocation.SymbolError.Symbol.FullName, symbolErrorWithLocation.SymbolError.GetErrorText() }.Concat(additionalArguments).ToArray();
        if (symbolErrorWithLocation.Location.HasValue())
        {
            return new Diagnostics(Diagnostic.Create(diagnosticDescriptor, symbolErrorWithLocation.Location, arguments));
        }

        return new Diagnostics(Diagnostic.Create(diagnosticDescriptor, Location.None, arguments));
    }

    public static Diagnostics Create(DiagnosticDescriptor diagnosticDescriptor, TypeSymbolWithLocation typeSymbolWithLocation, params object[] additionalArguments)
    {
        return Create(diagnosticDescriptor, typeSymbolWithLocation.TypeSymbol, typeSymbolWithLocation.Location, additionalArguments);
    }

    public static Diagnostics Create(DiagnosticDescriptor diagnosticDescriptor, ISymbol symbol, Location? location = default, params object[] additionalArguments)
    {
        var arguments = symbol.ToDisplayString().ToEnumerable().Concat(additionalArguments).ToArray();
        if (location.HasValue())
        {
            return new Diagnostics(Diagnostic.Create(diagnosticDescriptor, location, arguments));
        }

        if (symbol.DeclaringSyntaxReferences.IsEmpty)
        {
            return new Diagnostics(Diagnostic.Create(diagnosticDescriptor, Location.None, arguments));
        }

        return new Diagnostics(symbol.DeclaringSyntaxReferences.Select(x =>
            Diagnostic.Create(diagnosticDescriptor, x.GetSyntax().GetLocation(), arguments)));
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