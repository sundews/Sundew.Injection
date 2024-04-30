namespace OverallSuccessDependency;

using System;
using System.Threading.Tasks;

public class ManualSingletonDependencyFactory : IAsyncDisposable
{
    public ManualSingletonDependency ManualSingletonDependency { get; } = new();

    public async ValueTask DisposeAsync()
    {
        await this.ManualSingletonDependency.DisposeAsync();
    }
}