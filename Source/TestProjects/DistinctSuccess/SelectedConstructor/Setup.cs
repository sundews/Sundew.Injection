namespace DistinctSuccess.SelectedConstructor;

using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind(constructorSelector: () => new Dependency(default));

        injectionBuilder.ImplementFactory<RootFactory>();
    }
}

public class Root
{
    public Root(Dependency dependency)
    {
    }
}

public class Dependency
{
    public Dependency(int index)
    {
    }

    public Dependency(string name, int index)
    {
    }
}

public partial class RootFactory
{
    public partial Root Create(int index);
}