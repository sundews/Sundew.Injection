namespace Sundew.Injection.Generator.PerformanceTests;

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
    private readonly Compilation compilation;

    public InjectionGeneratorBenchmark()
    {
       this.compilation = TestProjects.AllFeatureSuccess.FromEntryAssembly.Value;
    }

    [Benchmark(Baseline = true)]
    public GeneratorDriverRunResult BaselineGenerator()
    {
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new DependenciesGenerator(), new BaselineInjectionGenerator());

        var driver = generatorDriver.RunGenerators(this.compilation!);
        var result = driver.GetRunResult();
        if (result.Diagnostics.Any())
        {
            var stringBuilder = new StringBuilder();
            foreach (var resultDiagnostic in result.Diagnostics)
            {
                stringBuilder.AppendLine(resultDiagnostic.GetMessage(CultureInfo.InvariantCulture));
            }

            throw new InvalidOperationException($"There should be no diagnostics: {stringBuilder}");
        }

        return result;
    }

    [Benchmark]
    public GeneratorDriverRunResult WorkInProgressGenerator()
    {
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new DependenciesGenerator(), new InjectionGenerator());
        var driver = generatorDriver.RunGenerators(this.compilation!);
        var result = driver.GetRunResult();
        if (result.Diagnostics.Any())
        {
            var stringBuilder = new StringBuilder();
            foreach (var resultDiagnostic in result.Diagnostics)
            {
                stringBuilder.AppendLine(resultDiagnostic.GetMessage(CultureInfo.InvariantCulture));
            }

            throw new InvalidOperationException($"There should be no diagnostics: {stringBuilder}");
        }

        return result;
    }
}