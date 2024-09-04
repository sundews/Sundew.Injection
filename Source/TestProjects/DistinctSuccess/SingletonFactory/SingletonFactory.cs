namespace DistinctSuccess.SingletonFactory;

using Sundew.Injection;

public class SingletonFactory : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<IBase, IRoot, Root>(Scope.SingleInstancePerFactory());

        injectionBuilder.ImplementFactory<RootFactory>(
            x => x
                .Add<IRoot>());
    }
}

public class Root : IRoot
{
}

public interface IRoot : IBase
{
}

public interface IBase
{
}

public partial class RootFactory
{
}