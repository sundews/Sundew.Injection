namespace Sundew.Injection.Testing;

using System;
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
        return WithPreviewLanguageVersion(project.Compile());
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
            return WithPreviewLanguageVersion(project.Compile());
        });

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
