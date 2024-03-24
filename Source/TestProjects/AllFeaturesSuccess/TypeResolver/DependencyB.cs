namespace AllFeaturesSuccess.TypeResolver;

public class DependencyB
{
    private readonly DependencyShared dependencyShared;

    public DependencyB(DependencyShared dependencyShared)
    {
        this.dependencyShared = dependencyShared;
    }
}