// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownAnalysisTypes.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System;
using System.Collections;
using Microsoft.CodeAnalysis;

public sealed class KnownAnalysisTypes : IKnownInjectableTypes
{
    public KnownAnalysisTypes(Compilation compilation)
    {
        this.IEnumerableTypeSymbol = compilation.GetTypeByMetadataName(typeof(IEnumerable).FullName) ?? throw new NotSupportedException("IEnumerable was not found");
        this.IDisposableTypeSymbol = compilation.GetTypeByMetadataName(typeof(IDisposable).FullName) ?? throw new NotSupportedException("IDisposable was not found");

        this.InjectionDeclarationType =
            compilation.GetTypeByMetadataName(typeof(IInjectionDeclaration).FullName) ?? throw new NotSupportedException("Target compilation must reference: Sundew.Injection");
        this.InjectionBuilderType =
            compilation.GetTypeByMetadataName(typeof(Injection.IInjectionBuilder).FullName) ?? throw new NotSupportedException("Target compilation must reference: Sundew.Injection");

        this.FactoryMethodSelectorTypeSymbol =
            compilation.GetTypeByMetadataName(typeof(IFactoryMethodSelector).FullName) ?? throw new NotSupportedException("Target compilation must reference: Sundew.Injection");

        this.FuncTypeSymbol =
            compilation.GetTypeByMetadataName(typeof(Func<>).FullName) ?? throw new NotSupportedException("Func<> was not found");
    }

    public INamedTypeSymbol FuncTypeSymbol { get; set; }

    public INamedTypeSymbol IEnumerableTypeSymbol { get; }

    public INamedTypeSymbol IDisposableTypeSymbol { get; }

    public INamedTypeSymbol InjectionDeclarationType { get; }

    public INamedTypeSymbol InjectionBuilderType { get; }

    public INamedTypeSymbol FactoryMethodSelectorTypeSymbol { get; }
}