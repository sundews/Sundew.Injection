namespace AllFeaturesSuccessDependency;

using Sundew.Injection;

public class FactoryDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.CreateFactory<DependencyFactory>(x => x.Add<Dependency>());
    }
}

public partial class DependencyFactory : IGeneratedFactory
{
}