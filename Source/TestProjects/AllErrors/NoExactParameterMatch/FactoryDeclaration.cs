namespace AllErrors.NoExactParameterMatch;

using Sundew.Injection;

public class FactoryDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.AddParameterProperties<IArguments>();
        injectionBuilder.AddParameter<Parameter>();

        injectionBuilder.ImplementFactory<MainFactory>(x => x.Add<Root>());
    }
}

public class MainFactory
{
}

public class Root
{
    public Root(Parameter parameter)
    {
    }
}

public class Parameter
{
}

public interface IArguments
{
    public Parameter NonMatchingParameter { get; set; }
}