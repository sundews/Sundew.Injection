namespace DistinctSuccess.Parameters.ConstructorOptionalReferenceType;

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
    private readonly IOption? option;

    public Root(IOption? option = default)
    {
        this.option = option;
    }
}

public interface IOption
{
}

public partial class RootFactory
{
    public static partial RootFactory Constructor([DefaultValue(default)] IOption? option);

    public partial Root Create();
}