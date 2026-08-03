namespace Sundew.Injection.Testing;

using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sundew.Base.IO;
using Sundew.Testing.CodeAnalysis;
using AssemblyReference = Sundew.Testing.CodeAnalysis.AssemblyReference;

public class TestProject(string path, params string[] additionalPaths)
{
    public Lazy<Compilation> FromCurrentDirectory { get; } = new(() =>
    {
        var project = new CSharpProject(
            FindPathUpwards(path),
            null,
            new Paths("bin", "obj"),
            new References(
                additionalPaths.Select(x => (IReference)new AssemblyReference(FindPathUpwards(x)))
                    .Concat([
                        new AssemblyReference(FindPathUpwards("Sundew.Injection.dll")),
                            new AssemblyReference(FindPathUpwards("Microsoft.Bcl.AsyncInterfaces.dll")),
                            new AssemblyReference(FindPathUpwards("Initialization.Interfaces.dll")),
                            new AssemblyReference(FindPathUpwards("Disposal.Interfaces.dll"))
                    ]).ToArray()));
        return WithPreviewLanguageVersion(project.Compile());
    });

    public Lazy<Compilation> FromEntryAssembly { get; } = new(() =>
        {
            var project = new CSharpProject(
                FindPathUpwards(path),
                null,
                new Paths("bin", "obj"),
                new References(
                    additionalPaths.Select(x => (IReference)new AssemblyReference(FindPathUpwards(x)))
                        .Concat([
                            new AssemblyReference(FindPathUpwards("Sundew.Injection.dll")),
                                new AssemblyReference(FindPathUpwards("Microsoft.Bcl.AsyncInterfaces.dll")),
                                new AssemblyReference(FindPathUpwards("Initialization.Interfaces.dll")),
                                new AssemblyReference(FindPathUpwards("Disposal.Interfaces.dll"))
                        ]).ToArray()));
            return WithPreviewLanguageVersion(project.Compile());
        });

    // Test paths are written with Windows separators, so they must be normalized to run on Linux and macOS too.
    private static string FindPathUpwards(string searchPath)
    {
        var normalizedPath = searchPath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        return Paths.FindPathUpwards(normalizedPath)
               ?? throw new DirectoryNotFoundException($"Could not find '{normalizedPath}' searching upwards from '{Directory.GetCurrentDirectory()}'.");
    }

    private static Compilation WithPreviewLanguageVersion(Compilation compilation)
    {
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
        var syntaxTrees = compilation.SyntaxTrees
            .Select(syntaxTree => CSharpSyntaxTree.ParseText(syntaxTree.GetText(), parseOptions, syntaxTree.FilePath))
            .ToArray();

        // The trees must be swapped in one go, because a compilation cannot contain trees with differing language versions.
        return compilation.RemoveAllSyntaxTrees().AddSyntaxTrees(syntaxTrees);
    }
}
