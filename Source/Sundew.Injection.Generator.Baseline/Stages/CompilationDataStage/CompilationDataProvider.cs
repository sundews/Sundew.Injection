// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilationDataProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CompilationDataStage;

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Sundew.Injection.Disposal;
using Sundew.Injection.Generator.TypeSystem;

public static class CompilationDataProvider
{
    private static readonly NamedType VoidType = new NamedType("void", string.Empty, string.Empty);

    public static IncrementalValueProvider<CompilationData> SetupCompilationInfoStage(this IncrementalValueProvider<Compilation> compilationProvider)
    {
        return compilationProvider.Select((compilation, _) => GetCompilationInfo(compilation));
    }

    internal static CompilationData GetCompilationInfo(Compilation compilation)
    {
        var areNullableAnnotationsSupported = compilation.Options.NullableContextOptions != NullableContextOptions.Disable;
        var disposingListType = compilation.GetTypeByMetadataName(typeof(Sundew.Injection.Disposal.DisposingList<>).FullName) ?? throw new NotSupportedException("Disposer was not found");
        var iDisposableType = CreateNamedType(compilation.GetTypeByMetadataName(typeof(IDisposable).FullName) ?? throw new NotSupportedException("IDisposable was not found"));
        var weakKeyDisposingDictionary = compilation.GetTypeByMetadataName(typeof(WeakKeyDisposingDictionary<>).FullName) ?? throw new NotSupportedException("WeakKeyDisposingDictionary was not found");
        var func = compilation.GetTypeByMetadataName(typeof(Func<>).FullName) ?? throw new NotSupportedException("Func was not found");
        return new CompilationData(
            areNullableAnnotationsSupported,
            GenericTypeConverter.GetGenericType(disposingListType).ToDefiniteBoundGenericType(ImmutableArray.Create(new DefiniteTypeArgument(iDisposableType, new TypeMetadata(null, true, false, false)))),
            iDisposableType,
            GenericTypeConverter.GetGenericType(weakKeyDisposingDictionary),
            VoidType,
            GenericTypeConverter.GetGenericType(func),
            compilation.AssemblyName ?? string.Empty);
    }

    private static NamedType CreateNamedType(INamedTypeSymbol namedTypeSymbol)
    {
        return new NamedType(namedTypeSymbol.MetadataName, TypeHelper.GetNamespace(namedTypeSymbol.ContainingNamespace), namedTypeSymbol.ContainingAssembly.Identity.ToString());
    }
}