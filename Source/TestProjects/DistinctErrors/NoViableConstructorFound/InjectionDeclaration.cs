namespace DistinctErrors.NoViableConstructorFound;

using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.BindGeneric<NoViableConstructor<object>>();
    }
}

public class NoViableConstructor<T>
{
    private NoViableConstructor()
    {
    }
}