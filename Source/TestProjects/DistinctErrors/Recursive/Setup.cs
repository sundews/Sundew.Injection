namespace DistinctErrors.Recursive;

using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<NewInstance>(Scope.NewInstance);
        injectionBuilder.Bind<Recursive>(Scope.SingleInstancePerFactory());

        injectionBuilder.ImplementFactory<RootFactory>();
    }
}

public partial class RootFactory
{
    public partial Root Create();
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