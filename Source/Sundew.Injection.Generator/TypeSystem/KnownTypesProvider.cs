// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownTypesProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System;
using System.Linq;
using System.Threading.Tasks;
using global::Initialization.Interfaces;
using Microsoft.CodeAnalysis;
using Sundew.Base;

public static class KnownTypesProvider
{
    public static readonly string BindableFactoryTargetName = typeof(BindableFactoryTargetAttribute).FullName!;
    public static readonly string IndirectFactoryTargetName = typeof(IndirectFactoryTargetAttribute).FullName!;

    public static R<INamedTypeSymbol, string> GetIInitializableTypeSymbol(this Compilation compilation)
    {
        return R.From(compilation.GetTypesByMetadataName(typeof(IInitializable).FullName!).FirstOrDefault(), () => "IInitializable was not found, Initialization.Interfaces must be referenced");
    }

    public static R<INamedTypeSymbol, string> GetIAsyncInitializableTypeSymbol(this Compilation compilation)
    {
        return R.From(compilation.GetTypesByMetadataName(typeof(IAsyncInitializable).FullName!).FirstOrDefault(), () => "IAsyncInitializable was not found, Initialization.Interfaces must be referenced");
    }

    public static R<INamedTypeSymbol, string> GetIDisposableTypeSymbol(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(IDisposable).FullName!), () => "IDisposable was not found");
    }

    public static R<INamedTypeSymbol, string> GetIAsyncDisposableTypeSymbol(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(IAsyncDisposable).FullName!), () => "IAsyncDisposable was not found");
    }

    public static R<INamedTypeSymbol, string> GetFunc(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(Func<>).FullName!), () => "Func<> was not found");
    }

    public static R<INamedTypeSymbol, string> GetTask(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(Task<>).FullName!), () => "Task<> was not found");
    }

    public static R<INamedTypeSymbol, string> GetConstructed(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(Sundew.Injection.Constructed<>).FullName!), () => "Constructed<> was not found");
    }

    public static R<INamedTypeSymbol, string> GetIEnumerableOfT(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(System.Collections.Generic.IEnumerable<>).FullName!), () => "IEnumerable<> was not found");
    }

    public static R<INamedTypeSymbol, string> GetIReadOnlyListOfT(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(System.Collections.Generic.IReadOnlyList<>).FullName!), () => "IReadOnlyList<> was not found");
    }
}