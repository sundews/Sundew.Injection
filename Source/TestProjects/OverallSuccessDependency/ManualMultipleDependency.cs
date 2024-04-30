namespace OverallSuccessDependency;

using System;
using System.Threading.Tasks;

public class ManualMultipleDependency : IAsyncDisposable, IIdentifiable
{
    public ManualMultipleDependency()
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