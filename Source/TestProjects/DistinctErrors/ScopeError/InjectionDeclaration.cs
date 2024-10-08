﻿namespace DistinctErrors.ScopeError;

using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<NewInstance>(Scope.NewInstance);
        injectionBuilder.Bind<SingleInstancePerFactory>(Scope.SingleInstancePerFactory());

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
    public SingleInstancePerFactory(NewInstance newInstance)
    {
    }
}

public class NewInstance
{
}