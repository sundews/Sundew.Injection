namespace Success.TypeResolver;

using SuccessDependency;

public class DependencyShared : IIdentifiable
{
    public DependencyShared()
    {
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }
}