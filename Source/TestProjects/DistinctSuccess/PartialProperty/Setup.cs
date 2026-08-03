namespace DistinctSuccess.PartialProperty;

using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<IRoot, Root>(Scope.SingleInstancePerFactory());

        injectionBuilder.ImplementFactory<RootFactory>();
    }
}

public class Root : IRoot
{
}

public interface IRoot
{
}

public partial class RootFactory
{
    public partial IRoot Root { get; }
}
