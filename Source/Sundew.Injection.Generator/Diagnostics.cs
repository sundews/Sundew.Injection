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
        "SI0001",
        Resources.UnknownErrorTitle,
        Resources.UnknownErrorMessageFormat,
        CodeGeneration,
        DiagnosticSeverity.Error,
        true,
        Resources.UnknownErrorDescription);

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
}