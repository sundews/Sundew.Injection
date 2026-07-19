namespace Sundew.Injection.Testing;

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Base.IO;
using Sundew.Testing.CodeAnalysis;
using AssemblyReference = Sundew.Testing.CodeAnalysis.AssemblyReference;

public class TestProject(string path, params string[] additionalPaths)
{
    public Lazy<Compilation> FromCurrentDirectory { get; } = new(() =>
    {
        var project = new CSharpProject(
            Paths.FindPathUpwards(path)!, //Path.Combine(path, "Recursive")
            null,
            new Paths("bin", "obj"),
            new References(
                additionalPaths.Select(x => (IReference)new AssemblyReference(Paths.FindPathUpwards(x)!))
                    .Concat([
                        new AssemblyReference(Paths.FindPathUpwards("Sundew.Injection.dll")!),
                            new AssemblyReference(Paths.FindPathUpwards("Microsoft.Bcl.AsyncInterfaces.dll")!),
                            new AssemblyReference(Paths.FindPathUpwards("Initialization.Interfaces.dll")!),
                            new AssemblyReference(Paths.FindPathUpwards("Disposal.Interfaces.dll")!)
                    ]).ToArray()));
        return project.Compile();
    });

    public Lazy<Compilation> FromEntryAssembly { get; } = new(() =>
        {
            var project = new CSharpProject(
                Paths.FindPathUpwards(path)!,
                null,
                new Paths("bin", "obj"),
                new References(
                    additionalPaths.Select(x => (IReference)new AssemblyReference(Paths.FindPathUpwards(x)!))
                        .Concat([
                            new AssemblyReference(Paths.FindPathUpwards("Sundew.Injection.dll")!),
                                new AssemblyReference(Paths.FindPathUpwards("Microsoft.Bcl.AsyncInterfaces.dll")!),
                                new AssemblyReference(Paths.FindPathUpwards("Initialization.Interfaces.dll")!),
                                new AssemblyReference(Paths.FindPathUpwards("Disposal.Interfaces.dll")!)
                        ]).ToArray()));
            return project.Compile();
        });
}
