// See https://aka.ms/new-console-template for more information

extern alias sundew;
using Result = sundew::Sundew.Base.Primitives.Computation.R;

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Sundew.CommandLine;
using Sundew.Injection.Generator.PerformanceTests;

var commandLineParser = new CommandLineParser<int, int>();

commandLineParser.WithArguments(new Arguments(new InfluxDbConnectionInfo()), arguments =>
{
    var summaries =
#if DEBUG
        BenchmarkRunner.Run(typeof(Program).Assembly, new DebugInProcessConfig());
#else
        BenchmarkRunner.Run(typeof(Program).Assembly, ManualConfig.Create(DefaultConfig.Instance));
#endif

    var isSuccess = summaries.All(x => x.Reports.All(x => x.Success));
    if (arguments.InfluxDbConnectionInfo != null)
    {
        using var influxDbClient = new InfluxDBClient(arguments.InfluxDbConnectionInfo.Uri, arguments.InfluxDbConnectionInfo.ApiKey);
        var now = DateTime.UtcNow;
        using var writeApi = influxDbClient.GetWriteApi();

        foreach (var summary in summaries)
        {
            foreach (var report in summary.Reports)
            {
                var point = PointData
                    .Measurement(report.BenchmarkCase.Descriptor.DisplayInfo)
                    .Tag("Runtime", report.BenchmarkCase.Job.Id)
                    .Tag("CPU", summary.HostEnvironmentInfo.CpuInfo.Value.ProcessorName)
                    .Tag("Configuration", summary.HostEnvironmentInfo.Configuration)
                    .Field("Allocated", report.Metrics.Where(x => x.Value.Descriptor.DisplayName == "Allocated").First().Value.Value)
                    .Field("Mean", report.ResultStatistics!.Mean)
                    .Field("StdDev", report.ResultStatistics.StandardDeviation)
                    .Field("StdErr", report.ResultStatistics.StandardError)
                    .Timestamp(now, WritePrecision.Ns);
                writeApi.WritePoint(point, arguments.InfluxDbConnectionInfo.Bucket, arguments.InfluxDbConnectionInfo.Organization);
                Console.WriteLine("Sent: " + point);
            }
        }
    }

    return Result.From(isSuccess, 0, ParserError.From(-1));
});

var result = await commandLineParser.ParseAsync(args);

if (!result.IsSuccess)
{
    result.WriteToConsole();
    return result.GetExitCode();
}

return 0;