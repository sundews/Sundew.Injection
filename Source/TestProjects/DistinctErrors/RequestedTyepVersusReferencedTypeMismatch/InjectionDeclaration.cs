namespace TestPlayground;

using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<Interface, Class>(Scope.SingleInstancePerFactory());

        injectionBuilder.ImplementFactory<MainFactory>(x => x
            .Add<Class>()
            .Add<Root>());
    }
}

public class Root
{
    public Root(NeedsClass needsClass, NeedsInterface needsInterface)
    {

    }
}

public class NeedsClass
{
    public NeedsClass(Class @class)
    {

    }
}

public class NeedsInterface
{
    public NeedsInterface(Interface @interface)
    {
    }
}

public partial class MainFactory
{
}

public class Class : Interface
{
}

public interface Interface
{
}