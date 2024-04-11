namespace Errors.NonGenericBind;

using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.BindGeneric<NonGenericType>();
    }
}

public class NonGenericType
{
}