namespace DistinctErrors.NonGenericBind;

using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.BindGeneric<NonGenericType>();
    }
}

public class NonGenericType
{
}