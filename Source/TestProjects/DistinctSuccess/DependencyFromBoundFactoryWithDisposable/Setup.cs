namespace DistinctSuccess.DependencyFromBoundFactoryWithDisposable;

using System;
using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.BindFactory<DependencyFactory>(x => x.Dependency);

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

public class DependencyFactory : IDisposable
{
    public Dependency Dependency { get; } = new Dependency();

    public void Dispose()
    {
    }
}

public class Dependency : IDisposable
{
    public void Dispose()
    {
    }
}