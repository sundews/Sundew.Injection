namespace AllFeaturesSuccess.TypeResolver;

public class DependencyA
{
    private readonly DependencyShared dependencyShared;

    public DependencyA(DependencyShared dependencyShared)
    {
        this.dependencyShared = dependencyShared;
    }
}