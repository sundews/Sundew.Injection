namespace Sundew.Injection.PerformanceTests;

extern alias baseline;
extern alias asyncinterfaces;
using BaselineInjectionGenerator = baseline::Sundew.Injection.Generator.InjectionGenerator;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Globalization;
using System.Text;
using System;
using Sundew.Injection.Testing;

#if NET6_0_OR_GREATER
#else
using IAsyncDisposable = asyncinterfaces::System.IAsyncDisposable;
#endif

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net48, baseline: true)]
[SimpleJob(RuntimeMoniker.Net70)]
public class InjectionGeneratorBenchmark
{
    private readonly Compilation compilation = TestProjects.AllFeatureSuccess.FromEntryAssembly.Value;

    [Benchmark(Baseline = true)]
    public GeneratorDriverRunResult BaselineGenerator()
    {
        return default!;
    }

    [Benchmark]
    public GeneratorDriverRunResult WorkInProgressGenerator()
    {
        return default!;
    }
}