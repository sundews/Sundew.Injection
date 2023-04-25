namespace Sundew.Injection.Generator.PerformanceTests;

using Sundew.CommandLine;

public class InfluxDbConnectionInfo : IArguments
{
    public InfluxDbConnectionInfo()
    :this(default!, default!, default!, default!)
    {
    }

    public InfluxDbConnectionInfo(string uri, string organization, string bucket, string apiKey)
    {
        this.Uri = uri;
        this.Organization = organization;
        this.Bucket = bucket;
        this.ApiKey = apiKey;
    }

    public string Uri { get; private set; }

    public string Bucket { get; private set; }

    public string Organization { get; private set; }

    public string ApiKey { get; private set; }

    public string HelpText { get; } = "Specify the values for InfluxDB connection.";

    public void Configure(IArgumentsBuilder argumentsBuilder)
    {
        argumentsBuilder.AddRequiredValue("Uri", () => this.Uri, x => this.Uri = x, "The uri of the InfluxDB server");
        argumentsBuilder.AddRequiredValue("Organization", () => this.Organization, x => this.Organization = x, "The organization.");
        argumentsBuilder.AddRequiredValue("Bucket", () => this.Bucket, x => this.Bucket = x, "The bucket.");
        argumentsBuilder.AddRequiredValue("Api-Key", () => this.ApiKey, x => this.ApiKey = x, "The ApiKey.");
    }
}