namespace DistinctErrors.NoBindingForNonInstantiableType;

using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.ImplementFactory<RootFactory>();
    }
}

public partial class RootFactory
{
    public partial IRoot Create();
}

public interface IRoot
{
}