namespace DistinctErrors.NoExactParameterMatch;

using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.AddParameterProperties<IArguments>();
        injectionBuilder.AddParameter<IParameter>();

        injectionBuilder.ImplementFactory<MainFactory>(x => x.Add<Root>());
    }
}

public partial class MainFactory
{
}

public class Root
{
    public Root(IParameter parameter)
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