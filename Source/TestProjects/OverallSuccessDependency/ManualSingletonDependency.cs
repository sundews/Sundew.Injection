namespace OverallSuccessDependency;

using System;
using System.Threading.Tasks;

public class ManualSingletonDependency : IAsyncDisposable, IIdentifiable
{
    public ManualSingletonDependency()
    {
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public ValueTask DisposeAsync()
    {
        FactoryLifetime.Disposed(this);
        return default;
    }
}