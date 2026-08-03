namespace DistinctErrors.RequestedTypeVersusReferencedTypeMismatch;

using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<Interface, Class>(Scope.SingleInstancePerFactory());

        injectionBuilder.ImplementFactory<MainFactory>();
    }
}

public partial class MainFactory
{
    public Interface? Class { get; }

    public partial Root CreateRoot();
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

public class Class : Interface
{
}

public interface Interface
{
}