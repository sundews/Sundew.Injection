namespace DistinctErrors.ImplementFactoryForNonNamedType;

using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.ImplementFactory<NamedType[]>(selector => selector.Add<Root>());
    }
}

public class NamedType
{
}

public class Root
{
}