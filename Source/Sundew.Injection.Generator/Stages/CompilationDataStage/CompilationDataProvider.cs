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
using Sundew.Injection.Generator.TypeSystem;

public static class CompilationDataProvider
{
    private static readonly NamedType VoidType = new NamedType("void", string.Empty, string.Empty);
    private static readonly NamedType ValueTaskType = new NamedType("ValueTask", "System.Threading.Tasks", string.Empty);

    public static IncrementalValueProvider<CompilationData> SetupCompilationInfoStage(this IncrementalValueProvider<Compilation> compilationProvider)
    {
        return compilationProvider.Select((compilation, _) => GetCompilationData(compilation));
    }

    internal static CompilationData GetCompilationData(Compilation compilation)
    {
        var areNullableAnnotationsSupported = compilation.Options.NullableContextOptions != NullableContextOptions.Disable;

        var iInitializableType = CreateNamedType(compilation.GetIInitializableTypeSymbol());
        var iAsyncInitializableType = CreateNamedType(compilation.GetIAsyncInitializableTypeSymbol());

        var iDisposableType = CreateNamedType(compilation.GetIDisposableTypeSymbol());
        var iAsyncDisposableType = CreateNamedType(compilation.GetIAsyncDisposableTypeSymbol());

        var func = compilation.GetFunc();
        var task = compilation.GetTask();
        return new CompilationData(
            areNullableAnnotationsSupported,
            iInitializableType,
            iAsyncInitializableType,
            iDisposableType,
            iAsyncDisposableType,
            GenericTypeConverter.GetGenericType(typeof(Sundew.Injection.Disposal.DisposingList<>), string.Empty).ToDefiniteBoundGenericType(ImmutableArray.Create(new DefiniteTypeArgument(iDisposableType, new TypeMetadata(null, false, false, false)))),
            GenericTypeConverter.GetGenericType(typeof(Sundew.Injection.Disposal.WeakKeyDisposingDictionary<>), string.Empty),
            new NamedType(nameof(ILifecycleHandler), typeof(Sundew.Injection.ILifecycleHandler).Namespace, string.Empty),
            new NamedType(nameof(LifecycleHandler), typeof(Sundew.Injection.LifecycleHandler).Namespace, string.Empty),
            GenericTypeConverter.GetGenericType(typeof(Sundew.Injection.Constructed<>), string.Empty),
            VoidType,
            ValueTaskType,
            GenericTypeConverter.GetGenericType(task),
            GenericTypeConverter.GetGenericType(func),
            compilation.AssemblyName ?? string.Empty);
    }

    private static NamedType CreateNamedType(INamedTypeSymbol namedTypeSymbol)
    {
        return new NamedType(namedTypeSymbol.MetadataName, TypeHelper.GetNamespace(namedTypeSymbol.ContainingNamespace), namedTypeSymbol.ContainingAssembly.Identity.ToString());
    }
}