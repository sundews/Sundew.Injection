namespace Sundew.Injection.Testing;

using System;
using Microsoft.CodeAnalysis;
using Sundew.Testing.CodeAnalysis;
using Sundew.Testing.IO;
using AssemblyReference = Sundew.Testing.CodeAnalysis.AssemblyReference;

public static class TestProjects
{
    public static Project AllFeatureSuccess = new Project(@"TestProjects/AllFeaturesSuccess");

    public class Project
    {
        public Project(string path)
        {
            this.FromEntryAssembly = new Lazy<Compilation>(() =>
            {
                var project = new CSharpProject(
                    Paths.FindPathUpwards(path),
                    null,
                    new Paths("bin", "obj"),
                    new References(
                        new AssemblyReference(Paths.FindPathUpwardsFromEntryAssembly("Sundew.Injection.dll")),
                        new AssemblyReference(Paths.FindPathUpwardsFromEntryAssembly("Microsoft.Bcl.AsyncInterfaces.dll")),
                        new AssemblyReference(Paths.FindPathUpwardsFromEntryAssembly("Initialization.Interfaces.dll")),
                        new AssemblyReference(Paths.FindPathUpwardsFromEntryAssembly("Disposal.Interfaces.dll"))));
                return project.Compile();
            });

            this.FromCurrentDirectory = new Lazy<Compilation>(() =>
            {
                var project = new CSharpProject(
                    Paths.FindPathUpwards(path),
                    null,
                    new Paths("bin", "obj"),
                    new References(
                        new AssemblyReference(Paths.FindPathUpwards("Sundew.Injection.dll")),
                        new AssemblyReference(Paths.FindPathUpwards("Microsoft.Bcl.AsyncInterfaces.dll")),
                        new AssemblyReference(Paths.FindPathUpwards("Initialization.Interfaces.dll")),
                        new AssemblyReference(Paths.FindPathUpwards("Disposal.Interfaces.dll"))));
                return project.Compile();
            });
        }

        public Lazy<Compilation> FromCurrentDirectory { get; set; }

        public Lazy<Compilation> FromEntryAssembly { get; set; }
    }
}