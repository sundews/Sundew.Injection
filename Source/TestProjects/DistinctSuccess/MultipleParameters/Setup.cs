namespace DistinctSuccess.MultipleParameters;
using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.ImplementFactory<RootFactory>();
    }
}


public interface IRequired1
{
}
public interface IRequired2
{
}

public class Root
{
    public Root(IRequired1 required1, IRequired2 required2)
    {

    }
}

public partial class RootFactory
{
    private static partial RootFactory Constructor(IRequired1 required1, IRequired2 required2);

    public partial Root Create();
}