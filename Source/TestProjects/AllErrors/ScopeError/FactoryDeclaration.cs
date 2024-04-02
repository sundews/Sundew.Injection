namespace AllErrors.ScopeError;

using Sundew.Injection;

public class FactoryDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<SingleInstancePerRequest>(Scope.SingleInstancePerRequest);
        injectionBuilder.Bind<SingleInstancePerFactory>(Scope.SingleInstancePerFactory);

        injectionBuilder.ImplementFactory<RootFactory>(selector => selector.Add<Root>());
    }
}

public class RootFactory
{
}

public class Root
{
    public Root(SingleInstancePerFactory singleInstancePerFactory)
    {
    }
}

public class SingleInstancePerFactory
{
    public SingleInstancePerFactory(SingleInstancePerRequest singleInstancePerRequest)
    {
    }
}

public class SingleInstancePerRequest
{
    public SingleInstancePerRequest(AutoInstance autoInstance)
    {
    }
}

public class AutoInstance
{
}