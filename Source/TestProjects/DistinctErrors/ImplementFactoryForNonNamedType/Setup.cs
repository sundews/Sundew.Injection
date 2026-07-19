namespace DistinctErrors.ImplementFactoryForNonNamedType;

using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.ImplementFactory<NamedType[]>(selector => selector.Add<Root>());
    }
}

public partial class NamedType
{
    public partial Root Create();
}

public class Root
{
}