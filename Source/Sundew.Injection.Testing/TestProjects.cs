namespace Sundew.Injection.Testing;

using System;
using Microsoft.CodeAnalysis;
using Sundew.Base.IO;
using Sundew.Testing.CodeAnalysis;
using AssemblyReference = Sundew.Testing.CodeAnalysis.AssemblyReference;

public static class TestProjects
{
    public static Project AllFeatureSuccess = new(@"TestProjects/Success");
    public static Project NetStandardLibraryErrors = new(@"TestProjects/Errors");
    public static Project TestPlayground = new(@"TestProjects/TestPlayground");

    public class Project(string path)
    {
        public Lazy<Compilation> FromCurrentDirectory { get; } = new Lazy<Compilation>(() =>
            {
                var project = new CSharpProject(
                    Paths.FindPathUpwards(path)!,
                    null,
                    new Paths("bin", "obj"),
                    new References(
                        new AssemblyReference(Paths.FindPathUpwards("SuccessDependency.dll")!),
                        new AssemblyReference(Paths.FindPathUpwards("Sundew.Injection.dll")!),
                        new AssemblyReference(Paths.FindPathUpwards("Microsoft.Bcl.AsyncInterfaces.dll")!),
                        new AssemblyReference(Paths.FindPathUpwards("Initialization.Interfaces.dll")!),
                        new AssemblyReference(Paths.FindPathUpwards("Disposal.Interfaces.dll")!)));
                return project.Compile();
            });

        public Lazy<Compilation> FromEntryAssembly { get; } = new Lazy<Compilation>(() =>
            {
                var project = new CSharpProject(
                    Paths.FindPathUpwards(path)!,
                    null,
                    new Paths("bin", "obj"),
                    new References(
                        new AssemblyReference(Paths.FindPathUpwardsFromEntryAssembly("SuccessDependency.dll")!),
                        new AssemblyReference(Paths.FindPathUpwardsFromEntryAssembly("Sundew.Injection.dll")!),
                        new AssemblyReference(Paths.FindPathUpwardsFromEntryAssembly("Microsoft.Bcl.AsyncInterfaces.dll")!),
                        new AssemblyReference(Paths.FindPathUpwardsFromEntryAssembly("Initialization.Interfaces.dll")!),
                        new AssemblyReference(Paths.FindPathUpwardsFromEntryAssembly("Disposal.Interfaces.dll")!)));
                return project.Compile();
            });
    }
}