namespace DistinctSuccess.FactoriesAllowingOverrideNew;

using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<Dependency>(isNewOverridable: true);

        injectionBuilder.ImplementFactory<MainFactory>(x => x.Add<Root>());
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
}


public class Dependency
{
}