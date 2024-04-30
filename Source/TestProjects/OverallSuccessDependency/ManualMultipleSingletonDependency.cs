namespace OverallSuccessDependency;

using System;
using System.Threading.Tasks;

public class ManualMultipleSingletonDependency : IAsyncDisposable, IIdentifiable
{
    public ManualMultipleSingletonDependency()
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