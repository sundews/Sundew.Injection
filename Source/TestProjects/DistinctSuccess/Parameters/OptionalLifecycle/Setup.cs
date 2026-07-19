namespace DistinctSuccess.Parameters.OptionalLifecycle;

using Initialization.Interfaces;
using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<IInitializationParameters, IDisposalParameters, ILifecycleParameters, LifecycleParameters>();

        injectionBuilder.ImplementFactory<RootFactory>();
    }
}

public class Root : IInitializable
{
    public Root(IOption? option = null)
    {
    }

    public void Initialize()
    {
    }
}

public interface IOption
{
}

public partial class RootFactory
{
    private static RootFactory Constructor(ILifecycleParameters? lifecycleParameters = null)
    {
        return new RootFactory(lifecycleParameters);
    }

    public partial Root Create();
}