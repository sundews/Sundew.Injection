namespace DistinctErrors.NoExactParameterMatch;

using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.ImplementFactory<MainFactory>();
    }
}

public partial class MainFactory
{
    public partial Root Create(IParameter parameter, IArguments arguments);
}

public class Root
{
    public Root(IParameter noMathParameter)
    {
    }
}

public interface IParameter
{
}

public interface IArguments
{
    public IParameter NonMatchingParameter { get; set; }
}