namespace OverallSuccessDependency;

using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.ImplementFactory<DependencyFactory>();
    }
}

public partial class DependencyFactory : IGeneratedFactory
{
    public partial Dependency Create();
}