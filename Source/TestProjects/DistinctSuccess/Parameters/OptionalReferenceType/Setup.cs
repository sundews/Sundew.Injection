namespace DistinctSuccess.Parameters.OptionalReferenceType;

using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.ImplementFactory<RootFactory>();
    }
}

public class Root
{
    public Root(IOption? option = null)
    {
    }
}

public interface IOption
{
}

public partial class RootFactory
{
    public partial Root Create(IOption? option = null);
}