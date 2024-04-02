namespace AllErrors.ImplementFactoryForNonNamedType;

using Sundew.Injection;

public class FactoryDeclaration : IInjectionDeclaration
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