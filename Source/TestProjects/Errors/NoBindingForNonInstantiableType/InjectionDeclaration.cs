namespace Errors.NoBindingForNonInstantiableType;

using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.ImplementFactory<RootFactory>(selector => selector.Add<IRoot>());
    }
}

public class RootFactory
{
}

public interface IRoot
{
}