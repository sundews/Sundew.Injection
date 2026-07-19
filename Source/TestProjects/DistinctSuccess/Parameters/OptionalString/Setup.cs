namespace DistinctSuccess.Parameters.OptionalString;

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
    public Root(string? name = null)
    {
    }
}

public interface IOption
{
}

public partial class RootFactory
{
    public partial Root Create(string? name = null);
}