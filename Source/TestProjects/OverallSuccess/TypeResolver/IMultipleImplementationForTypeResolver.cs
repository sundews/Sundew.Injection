namespace OverallSuccess.TypeResolver;

public interface IMultipleImplementationForTypeResolver
{
    
}

public class MultipleImplementationForTypeResolverA : IMultipleImplementationForTypeResolver
{
    private readonly DependencyA dependencyA;

    public MultipleImplementationForTypeResolverA(DependencyA dependencyA)
    {
        this.dependencyA = dependencyA;
    }
}

public class MultipleImplementationForTypeResolverB : IMultipleImplementationForTypeResolver
{
    private readonly DependencyB dependencyB;

    public MultipleImplementationForTypeResolverB(DependencyB dependencyB)
    {
        this.dependencyB = dependencyB;
    }
}

public class MultipleImplementationForTypeResolverC : IMultipleImplementationForTypeResolver
{
}