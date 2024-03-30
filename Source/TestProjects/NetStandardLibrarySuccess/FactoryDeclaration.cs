
namespace NetStandardLibrarySuccess;

using Sundew.Injection;

public class FactoryDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.CreateFactory<TFactory>(x => x.Add<T>());
    }
}

public partial class TFactory
{
}

public class T
{
}
