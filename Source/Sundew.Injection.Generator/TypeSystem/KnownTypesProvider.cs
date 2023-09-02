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

public static class KnownTypesProvider
{
    public static INamedTypeSymbol GetIInitializableTypeSymbol(this Compilation compilation)
    {
        return compilation.GetTypeByMetadataName(typeof(IInitializable).FullName) ?? throw new NotSupportedException("IInitializable was not found, Initialization.Interfaces must be referenced");
    }

    public static INamedTypeSymbol GetIAsyncInitializableTypeSymbol(this Compilation compilation)
    {
        return compilation.GetTypeByMetadataName(typeof(IAsyncInitializable).FullName) ?? throw new NotSupportedException("IAsyncInitializable was not found, Initialization.Interfaces must be referenced");
    }

    public static INamedTypeSymbol GetIDisposableTypeSymbol(this Compilation compilation)
    {
        return compilation.GetTypeByMetadataName(typeof(IDisposable).FullName) ?? throw new NotSupportedException("IDisposable was not found");
    }

    public static INamedTypeSymbol GetIAsyncDisposableTypeSymbol(this Compilation compilation)
    {
        return compilation.GetTypeByMetadataName(typeof(IAsyncDisposable).FullName) ?? throw new NotSupportedException("IAsyncDisposable was not found");
    }

    public static INamedTypeSymbol GetFunc(this Compilation compilation)
    {
        return compilation.GetTypeByMetadataName(typeof(Func<>).FullName) ?? throw new NotSupportedException("Func<> was not found");
    }

    public static INamedTypeSymbol GetTask(this Compilation compilation)
    {
        return compilation.GetTypeByMetadataName(typeof(Task<>).FullName) ?? throw new NotSupportedException("Task<> was not found");
    }

    public static INamedTypeSymbol GetLifecycleHandler(this Compilation compilation)
    {
        return compilation.GetTypesByMetadataName(typeof(LifecycleHandler).FullName).FirstOrDefault() ?? throw new NotSupportedException("LifecycleHandler was not found");
    }

    public static INamedTypeSymbol GetILifecycleHandler(this Compilation compilation)
    {
        return compilation.GetTypesByMetadataName(typeof(ILifecycleHandler).FullName).FirstOrDefault() ?? throw new NotSupportedException("ILifecycleHandler was not found");
    }

    public static INamedTypeSymbol GetConstructed(this Compilation compilation)
    {
        return compilation.GetTypeByMetadataName(typeof(Sundew.Injection.Constructed<>).FullName) ?? throw new NotSupportedException("Constructed<> was not found");
    }
}