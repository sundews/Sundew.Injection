namespace OverallSuccessDependency;

public class ManualMultipleDependencyFactory
{
    public ManualMultipleSingletonDependency ManualMultipleSingletonDependency { get; } = new();

    // public int Id { get; }

    public ManualMultipleDependency CreateNewInstance()
    {
        return new ManualMultipleDependency();
    }
}