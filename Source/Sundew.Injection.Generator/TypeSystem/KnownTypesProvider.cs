namespace Sundew.Injection.Generator.TypeSystem;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

public static class KnownTypesProvider
{
    private const string IInitializable = "Initialization.Interfaces.IInitializable";
    private const string IAsyncInitializable = "Initialization.Interfaces.IAsyncInitializable";
    private const string IAsyncDisposable = "System.IAsyncDisposable";
    private const string InitializationInterfaces = "Initialization.Interfaces";
    private const string SundewInjectionGenerator = "Sundew.Injection.Generator";

    public static INamedTypeSymbol GetIInitializableTypeSymbol(this Compilation compilation)
    {
        return compilation.GetTypesByMetadataName(IInitializable).FirstOrDefault(x => x.ContainingAssembly.Name == InitializationInterfaces) ?? throw new NotSupportedException("IInitializable was not found, Initialization.Interfaces must be referenced");
    }

    public static INamedTypeSymbol GetIAsyncInitializableTypeSymbol(this Compilation compilation)
    {
        return compilation.GetTypesByMetadataName(IAsyncInitializable).FirstOrDefault(x => x.ContainingAssembly.Name == InitializationInterfaces) ?? throw new NotSupportedException("IAsyncInitializable was not found, Initialization.Interfaces must be referenced");
    }

    public static INamedTypeSymbol GetIDisposableTypeSymbol(this Compilation compilation)
    {
        return compilation.GetTypeByMetadataName(typeof(IDisposable).FullName) ?? throw new NotSupportedException("IDisposable was not found");
    }

    public static INamedTypeSymbol GetIAsyncDisposableTypeSymbol(this Compilation compilation)
    {
        return compilation.GetTypesByMetadataName(IAsyncDisposable).FirstOrDefault(x => x.ContainingAssembly.Name != SundewInjectionGenerator) ?? throw new NotSupportedException("IAsyncDisposable was not found");
    }

    public static INamedTypeSymbol GetFunc(this Compilation compilation)
    {
        return compilation.GetTypeByMetadataName(typeof(Func<>).FullName) ?? throw new NotSupportedException("Func<> was not found");
    }

    public static INamedTypeSymbol GetTask(this Compilation compilation)
    {
        return compilation.GetTypeByMetadataName(typeof(Task<>).FullName) ?? throw new NotSupportedException("Task<> was not found");
    }
}