namespace DistinctSuccess.InitializableDependency;

using Initialization.Interfaces;
using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.ImplementFactory<MainFactory>();
    }
}

public partial class MainFactory
{
    public partial Root Create();
}

public class Root
{
    public Root(Dependency dependency)
    {

    }
}
public class Dependency : IInitializable
{
    public void Initialize()
    {
    }
}