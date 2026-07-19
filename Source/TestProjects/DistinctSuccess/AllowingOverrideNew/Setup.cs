namespace DistinctSuccess.AllowingOverrideNew;

using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<Dependency>(isNewOverridable: true);

        injectionBuilder.ImplementFactory<MainFactory>();
    }
}

public class Root
{
    public Root(Dependency dependency)
    {

    }
}

public partial class MainFactory
{
    public partial Root Create();
}


public class Dependency
{
}