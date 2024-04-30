namespace OverallSuccess.TypeResolver;

using OverallSuccessDependency;

public class DependencyB : IIdentifiable
{
    private readonly DependencyShared dependencyShared;

    public DependencyB(DependencyShared dependencyShared)
    {
        this.dependencyShared = dependencyShared;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }
}