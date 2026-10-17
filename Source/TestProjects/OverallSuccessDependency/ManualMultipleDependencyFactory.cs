namespace OverallSuccessDependency;

using System.Collections.Generic;
using System.Threading.Tasks;

public class ManualMultipleDependencyFactory
{
    public ManualMultipleSingletonDependency ManualMultipleSingletonDependency { get; } = new();

    // public int Id { get; }

    public List<object> DisposedInstances { get; } = new();

    public ManualMultipleDependency CreateNewInstance()
    {
        return new ManualMultipleDependency();
    }

    public void Dispose(ManualMultipleDependency manualMultipleDependency)
    {
        this.DisposedInstances.Add(manualMultipleDependency);
    }

    public ValueTask DisposeAsync(ManualMultipleSingletonDependency manualMultipleSingletonDependency)
    {
        this.DisposedInstances.Add(manualMultipleSingletonDependency);
        return default;
    }
}