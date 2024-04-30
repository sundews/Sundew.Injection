namespace OverallSuccessDependency;

using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.ImplementFactory<DependencyFactory>(x => x.Add<Dependency>());
    }
}

public partial class DependencyFactory : IGeneratedFactory
{
}