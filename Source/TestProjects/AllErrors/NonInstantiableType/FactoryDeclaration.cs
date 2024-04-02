namespace AllErrors.NonInstantiableType;

using Sundew.Injection;

public class FactoryDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.BindGeneric<AbstractGenericType<object>>();
    }
}

public abstract class AbstractGenericType<T>
{
}