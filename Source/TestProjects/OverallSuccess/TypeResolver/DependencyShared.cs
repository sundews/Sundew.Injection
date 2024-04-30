namespace OverallSuccess.TypeResolver;

using OverallSuccessDependency;

public class DependencyShared : IIdentifiable
{
    public DependencyShared()
    {
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }
}