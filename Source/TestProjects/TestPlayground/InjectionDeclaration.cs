namespace TestPlayground;

using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.ImplementFactory<MainFactory>(x => x.Add<Root>());
    }
}

public class Root
{
}

public partial class MainFactory
{
}