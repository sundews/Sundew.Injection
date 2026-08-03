namespace DistinctSuccess.Parameters.OptionalInt;

using Sundew.Injection;

public class FactoryWithOptionalIntToRequiredParameter : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.ImplementFactory<RootFactory>();
    }
}

public class Root
{
    public Root(int? index = 3)
    {
    }
}

public interface IOption
{
}

public partial class RootFactory
{
    public partial Root Create(int? index = 3);
}