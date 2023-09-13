namespace AllFeaturesSuccessDependency;

using System;
using System.Threading.Tasks;

public class ManualDependency : IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return default;
    }
}