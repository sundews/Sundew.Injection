namespace Sundew.Injection.Generator.PerformanceTests;

using Sundew.CommandLine;

public class Arguments : IArguments
{
    public Arguments()
    : this(null)
    {
        
    }
    public Arguments(InfluxDbConnectionInfo? influxDbConnectionInfo)
    {
        this.InfluxDbConnectionInfo = influxDbConnectionInfo;
    }

    public InfluxDbConnectionInfo? InfluxDbConnectionInfo { get; private set; }

    public string HelpText { get; } = "Runs the performance tests and reports results";

    public void Configure(IArgumentsBuilder argumentsBuilder)
    {
        argumentsBuilder.AddOptional("ib", "influx-db", this.InfluxDbConnectionInfo, () => new InfluxDbConnectionInfo(),
            info => this.InfluxDbConnectionInfo = info, "The InfluxDB connection info.");
    }
}