namespace DistinctErrors.Recursive;

using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<NewInstance>(Scope.NewInstance);
        injectionBuilder.Bind<Recursive>(Scope.SingleInstancePerFactory());

        injectionBuilder.ImplementFactory<RootFactory>(selector => selector.Add<Root>());
    }
}

public class RootFactory
{
}

public class Root
{
    public Root(Recursive recursive)
    {
    }
}

public class Recursive
{
    public Recursive(NewInstance newInstance)
    {
    }
}

public class NewInstance
{
    public NewInstance(Recursive recursive)
    {
    }
}