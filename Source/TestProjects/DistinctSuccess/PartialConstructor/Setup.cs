namespace DistinctSuccess.PartialConstructor;

using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.ImplementFactory<RootFactory>();
    }
}

public class Root
{
    public Root(Dependency dependency)
    {
    }
}

public class Dependency;

public partial class RootFactory
{
    public partial RootFactory(Dependency dependency);

    public partial Root Create();
}
