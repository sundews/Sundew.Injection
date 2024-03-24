// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownTypesProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using global::Initialization.Interfaces;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

public static class KnownTypesProvider
{
    public static readonly string BindableCreateMethodName = typeof(BindableCreateMethodAttribute).FullName!;
    public static readonly string IndirectCreateMethodName = typeof(IndirectCreateMethodAttribute).FullName!;

    public static R<INamedTypeSymbol, string> GetIInitializableTypeSymbol(this Compilation compilation)
    {
        return R.From(compilation.GetTypesByMetadataName(typeof(IInitializable).FullName).FirstOrDefault(), () => "IInitializable was not found, Initialization.Interfaces must be referenced");
    }

    public static R<INamedTypeSymbol, string> GetIAsyncInitializableTypeSymbol(this Compilation compilation)
    {
        return R.From(compilation.GetTypesByMetadataName(typeof(IAsyncInitializable).FullName).FirstOrDefault(), () => "IAsyncInitializable was not found, Initialization.Interfaces must be referenced");
    }

    public static R<INamedTypeSymbol, string> GetIDisposableTypeSymbol(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(IDisposable).FullName), () => "IDisposable was not found");
    }

    public static R<INamedTypeSymbol, string> GetIAsyncDisposableTypeSymbol(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(IAsyncDisposable).FullName), () => "IAsyncDisposable was not found");
    }

    public static R<INamedTypeSymbol, string> GetFunc(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(Func<>).FullName), () => "Func<> was not found");
    }

    public static R<INamedTypeSymbol, string> GetTask(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(Task<>).FullName), () => "Task<> was not found");
    }

    public static R<INamedTypeSymbol, string> GetLifecycleHandler(this Compilation compilation)
    {
        return R.From(compilation.GetTypesByMetadataName(typeof(LifecycleHandler).FullName).FirstOrDefault(), () => "LifecycleHandler was not found");
    }

    public static R<INamedTypeSymbol, string> GetResolverItemsFactory(this Compilation compilation)
    {
        return R.From(compilation.GetTypesByMetadataName(typeof(ResolverItemsFactory).FullName).FirstOrDefault(), () => "ResolverItemsFactoryType was not found");
    }

    public static R<INamedTypeSymbol, string> GetResolverItem(this Compilation compilation)
    {
        return R.From(compilation.GetTypesByMetadataName(typeof(ResolverItem).FullName).FirstOrDefault(), () => "ResolverItemType was not found");
    }

    public static R<INamedTypeSymbol, string> GetILifecycleHandler(this Compilation compilation)
    {
        return R.From(compilation.GetTypesByMetadataName(typeof(ILifecycleHandler).FullName).FirstOrDefault(), () => "ILifecycleHandler was not found");
    }

    public static R<INamedTypeSymbol, string> GetConstructed(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(Sundew.Injection.Constructed<>).FullName), () => "Constructed<> was not found");
    }

    public static R<INamedTypeSymbol, string> GetIEnumerableOfT(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(System.Collections.Generic.IEnumerable<>).FullName), () => "IEnumerable<> was not found");
    }

    public static R<INamedTypeSymbol, string> GetIReadOnlyListOfT(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(System.Collections.Generic.IReadOnlyList<>).FullName), () => "IReadOnlyList<> was not found");
    }
}