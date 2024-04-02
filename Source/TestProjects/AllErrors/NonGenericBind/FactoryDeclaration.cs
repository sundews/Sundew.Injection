namespace AllErrors.NonGenericBind;

using Sundew.Injection;

public class FactoryDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.BindGeneric<NonGenericType>();
    }
}

public class NonGenericType
{
}