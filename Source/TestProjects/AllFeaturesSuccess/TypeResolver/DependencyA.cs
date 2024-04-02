namespace AllFeaturesSuccess.TypeResolver;

using AllFeaturesSuccessDependency;

public class DependencyA : IIdentifiable
{
    private readonly DependencyShared dependencyShared;

    public DependencyA(DependencyShared dependencyShared)
    {
        this.dependencyShared = dependencyShared;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }
}