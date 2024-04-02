//HintName: AllFeaturesSuccess.IMultipleImplementationForTypeResolverFactory.generated.cs
#nullable enable
namespace AllFeaturesSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    public partial interface IMultipleImplementationForTypeResolverFactory : global::Sundew.Injection.IGeneratedFactory
    {
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        global::AllFeaturesSuccess.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverA();

        [global::Sundew.Injection.BindableCreateMethodAttribute]
        global::AllFeaturesSuccess.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverB();
    }
}
