namespace AllErrors.NoViableConstructorFound;

using Sundew.Injection;

public class FactoryDeclaration : IInjectionDeclaration
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