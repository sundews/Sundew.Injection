namespace DistinctErrors.RequestedTypeVsOptionalParameter;

using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.ImplementFactory<RootFactory>();
    }
}

public partial class RootFactory
{
    private static partial RootFactory Create([DefaultValue(default)] IParameter? parameter);

    public partial Root Create();
}

public class Root
{
    public Root(Dependency1 dependency1, Dependency2 dependency2)
    {
    }
}

public class Dependency1
{
    public Dependency1(IParameter? parameter)
    {
    }
}

public class Dependency2
{
    public Dependency2(IParameter parameter)
    {
    }
}

public interface IParameter
{
}