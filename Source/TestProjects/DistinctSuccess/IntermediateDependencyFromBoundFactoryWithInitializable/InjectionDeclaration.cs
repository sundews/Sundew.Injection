namespace DistinctSuccess.IntermediateDependencyFromBoundFactoryWithInitializable;

using System;
using Initialization.Interfaces;
using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.BindFactory<DependencyFactory>(x => x.Dependency);

        injectionBuilder.ImplementFactory<MainFactory>(x => x.Add<Root>());
    }
}

public partial class MainFactory;

public class Root
{
    public Root(Intermediate intermediate)
    {

    }
}

public class Intermediate : IInitializable
{
    public Intermediate(Dependency dependency)
    {
    }

    public void Initialize()
    {
    }
}

public class DependencyFactory : IDisposable
{
    public Dependency Dependency { get; } = new Dependency();

    public void Dispose()
    {
    }
}

public class Dependency : IInitializable
{
    public void Initialize()
    {
    }
}