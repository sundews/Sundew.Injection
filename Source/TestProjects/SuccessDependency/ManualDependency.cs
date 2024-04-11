namespace SuccessDependency;

using System;
using System.Threading.Tasks;

public class ManualDependency : IAsyncDisposable, IIdentifiable
{
    public ManualDependency()
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