namespace DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration;

using System;
using Initialization.Interfaces;
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

public class Root : IInitializable, IDisposable
{
    public Root(IRequired1 required1, IRequired2 required2)
    {

    }

    public void Initialize()
    {
    }

    public void Dispose()
    {
    }
}

public partial class RootFactory
{
    private static partial RootFactory Constructor(IRequired1 required1, IRequired2 required2, [DefaultValue(default)] ILifecycleParameters? lifecycleParameters);

    public partial Root Create();
}