namespace SuccessDependency;

public class ManualDependencyFactory
{
    public ManualDependencyFactory()
    {
        // this.Id = FactoryLifetime.Created(this);
    }

    // public int Id { get; }

    public ManualDependency Create()
    {
        return new ManualDependency();
    }
}