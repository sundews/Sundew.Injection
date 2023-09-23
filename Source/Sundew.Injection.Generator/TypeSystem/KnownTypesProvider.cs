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
using Sundew.Base.Primitives.Computation;

public static class KnownTypesProvider
{
    public static R<INamedTypeSymbol, string> GetIInitializableTypeSymbol(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(IInitializable).FullName), () => "IInitializable was not found, Initialization.Interfaces must be referenced");
    }

    public static R<INamedTypeSymbol, string> GetIAsyncInitializableTypeSymbol(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(IAsyncInitializable).FullName), () => "IAsyncInitializable was not found, Initialization.Interfaces must be referenced");
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

    public static R<INamedTypeSymbol, string> GetTypeResolver(this Compilation compilation)
    {
        return R.From(compilation.GetTypesByMetadataName(typeof(TypeResolver<>).FullName).FirstOrDefault(), () => "TypeResolver<> was not found");
    }

    public static R<INamedTypeSymbol, string> GetILifecycleHandler(this Compilation compilation)
    {
        return R.From(compilation.GetTypesByMetadataName(typeof(ILifecycleHandler).FullName).FirstOrDefault(), () => "ILifecycleHandler was not found");
    }

    public static R<INamedTypeSymbol, string> GetConstructed(this Compilation compilation)
    {
        return R.From(compilation.GetTypeByMetadataName(typeof(Sundew.Injection.Constructed<>).FullName), () => "Constructed<> was not found");
    }
}