// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Diagnostics.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator;

using Microsoft.CodeAnalysis;

public static class Diagnostics
{
    private const string CodeGeneration = "CodeGeneration";

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

    public static DiagnosticDescriptor ResolveTypeError { get; } = new(
        "SI0004",
        Resources.ResolveTypeErrorTitle,
        Resources.ResolveTypeErrorMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.ResolveTypeErrorDescription);

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
}