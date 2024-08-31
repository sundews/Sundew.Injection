namespace TestPlayground;

using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        // injectionBuilder.ImplementFactory<MainFactory>();
    }
}

public partial class MainFactory
{
    private static partial MainFactory Constructor(ConstructorParameter constructorParameter);

    public CreatedSingleInstance CreatedSingleInstance { get; }

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

/// <summary>
/// Generated
/// </summary>
public partial class MainFactory
{
    private readonly ConstructorParameter constructorParameter;

    public MainFactory(ConstructorParameter constructorParameter)
    {
        this.constructorParameter = constructorParameter;
        this.CreatedSingleInstance = new CreatedSingleInstance();
    }

    private static partial MainFactory Constructor(ConstructorParameter constructorParameter)
    {
        return new MainFactory(constructorParameter);
    }

    public partial Root Create(FactoryMethodParameter factoryMethodParameter)
    {
        return new Root(this.constructorParameter, factoryMethodParameter);
    }
}