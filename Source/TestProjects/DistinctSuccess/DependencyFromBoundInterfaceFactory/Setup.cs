namespace DistinctSuccess.DependencyFromBoundInterfaceFactory;

using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<IFactory, Factory>(Scope.SingleInstancePerFactory());

        injectionBuilder.BindFactory<IFactory>(x => x.Add(x => x.Create()));

        injectionBuilder.ImplementFactory<RootFactory>();
    }
}

public partial class RootFactory
{
    public partial Root Create();
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