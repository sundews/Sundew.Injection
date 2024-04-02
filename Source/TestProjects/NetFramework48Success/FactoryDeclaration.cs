
namespace NetFramework48Success;

using System.Collections.Generic;
using Sundew.Injection;

public class FactoryDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<IAbstract, Concrete1>();
        injectionBuilder.Bind<IAbstract, Concrete2>();
        injectionBuilder.ImplementFactory<MainFactory>(x => x.Add<Root>());
    }
}

public partial class MainFactory
{
}

public class Root
{
    private readonly IEnumerable<IAbstract> abstracts;

    public Root(IEnumerable<IAbstract> abstracts)
    {
        this.abstracts = abstracts;
    }
}

public interface IAbstract
{
}

public class Concrete1 : IAbstract { }
public class Concrete2 : IAbstract { }
