namespace AllFeaturesSuccessDependency
{
    using Sundew.Injection;

    public class FactoryDeclaration : IInjectionDeclaration
    {
        public void Configure(IInjectionBuilder injectionBuilder)
        {
            injectionBuilder.CreateFactory<Dependency>();
        }
    }
}
