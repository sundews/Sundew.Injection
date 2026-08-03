namespace DistinctSuccess.Parameters.OptionalIntToRequired;

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
    public Root(int index)
    {
    }
}

public interface IOption
{
}

public partial class RootFactory
{
    public partial Root Create(int? index);
}