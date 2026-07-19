namespace DistinctErrors.ScopeError;

using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<NewInstance>(Scope.NewInstance);
        injectionBuilder.Bind<SingleInstancePerFactory>(Scope.SingleInstancePerFactory());

        injectionBuilder.ImplementFactory<RootFactory>();
    }
}

public partial class RootFactory
{
    public partial Root Create();
}

public class Root
{
    public Root(SingleInstancePerFactory singleInstancePerFactory)
    {
    }
}

public class SingleInstancePerFactory
{
    public SingleInstancePerFactory(NewInstance newInstance)
    {
    }
}

public class NewInstance
{
}
