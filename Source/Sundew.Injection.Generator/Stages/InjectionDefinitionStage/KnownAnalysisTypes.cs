﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownAnalysisTypes.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using System;
using System.Collections;
using Microsoft.CodeAnalysis;
using Sundew.Injection.Generator.TypeSystem;

public sealed class KnownAnalysisTypes : IKnownInjectableTypes
{
    private const string SundewInjection = "Sundew.Injection";

    public KnownAnalysisTypes(Compilation compilation)
    {
        this.IEnumerableTypeSymbol = compilation.GetTypeByMetadataName(typeof(IEnumerable).FullName) ?? throw new NotSupportedException("IEnumerable was not found");

        this.IInitializableTypeSymbol = compilation.GetIInitializableTypeSymbol();
        this.IAsyncInitializableTypeSymbol = compilation.GetIAsyncInitializableTypeSymbol();

        this.IDisposableTypeSymbol = compilation.GetIDisposableTypeSymbol();
        this.IAsyncDisposableTypeSymbol = compilation.GetIAsyncDisposableTypeSymbol();

        this.InjectionDeclarationType =
            compilation.TryGetTypeByAssemblyQualifiedMetadataName(typeof(IInjectionDeclaration), SundewInjection) ?? throw new NotSupportedException("IInjectionDeclaration was not found, Sundew.Injection must be referenced");
        this.InjectionBuilderType =
            compilation.TryGetTypeByAssemblyQualifiedMetadataName(typeof(IInjectionBuilder), SundewInjection) ?? throw new NotSupportedException("IInjectionBuilder was not found, Sundew.Injection must be referenced");

        this.FactoryMethodSelectorTypeSymbol =
            compilation.TryGetTypeByAssemblyQualifiedMetadataName(typeof(IFactoryMethodSelector), SundewInjection) ?? throw new NotSupportedException("IFactoryMethodSelector was not found, Sundew.Injection must be referenced");

        this.FuncTypeSymbol = compilation.GetFunc();
    }

    public INamedTypeSymbol FuncTypeSymbol { get; set; }

    public INamedTypeSymbol IEnumerableTypeSymbol { get; }

    public INamedTypeSymbol IDisposableTypeSymbol { get; }

    public INamedTypeSymbol IAsyncDisposableTypeSymbol { get; }

    public INamedTypeSymbol IInitializableTypeSymbol { get; }

    public INamedTypeSymbol IAsyncInitializableTypeSymbol { get; }

    public INamedTypeSymbol InjectionDeclarationType { get; }

    public INamedTypeSymbol InjectionBuilderType { get; }

    public INamedTypeSymbol FactoryMethodSelectorTypeSymbol { get; }
}