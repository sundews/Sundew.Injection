namespace TestPlayground;

using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.ImplementFactory<MainFactory>();
    }
}

public partial class MainFactory
{
    public partial MainFactory(ConstructorParameter constructorParameter);

    public partial CreatedSingleInstance CreatedSingleInstance { get; }

    public partial Root Create(FactoryMethodParameter factoryMethodParameter);
}

public class Root
{
    public Root(ConstructorParameter constructorParameter, FactoryMethodParameter factoryMethodParameter)
    {
    }
}

public class FactoryMethodParameter;

public class ConstructorParameter;

public class CreatedSingleInstance;