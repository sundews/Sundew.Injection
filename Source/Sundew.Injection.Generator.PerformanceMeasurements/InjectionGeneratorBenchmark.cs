namespace Sundew.Injection.Generator.PerformanceMeasurements;

using System.Globalization;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sundew.Injection.Testing;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net48, baseline: true)]
[SimpleJob(RuntimeMoniker.Net70)]
[JsonExporterAttribute.Brief]
[MarkdownExporterAttribute.GitHub]
[RankColumn]
public class InjectionGeneratorBenchmark
{
    private readonly Compilation compilation;

    public InjectionGeneratorBenchmark()
    {
        this.compilation = TestProjects.AllFeatureSuccess.FromEntryAssembly.Value;
    }

    [Benchmark]
    public GeneratorDriverRunResult GenerateDemoApp()
    {
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new InjectionGenerator());

        var driver= generatorDriver.RunGenerators(this.compilation);
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