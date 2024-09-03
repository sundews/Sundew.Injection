namespace DistinctSuccess.DependencyFromBoundInterfaceFactory;

using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<IFactory, Factory>(Scope.SingleInstancePerFactory());

        injectionBuilder.BindFactory<IFactory>(x => x.Add(x => x.Create()));

        injectionBuilder.ImplementFactory<RootFactory>(x => x.Add<Root>());
    }
}

public partial class RootFactory
{
}

public class Root
{
    public Root(Dependency dependency)
    {

    }
}

public class Factory : IFactory
{
    public Dependency Create()
    {
        return new Dependency();
    }
}

public interface IFactory
{
    Dependency Create();
}

public class Dependency
{
}