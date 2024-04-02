namespace AllFeaturesSuccess.TypeResolver;

using AllFeaturesSuccessDependency;

public class DependencyShared : IIdentifiable
{
    public DependencyShared()
    {
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }
}