//HintName: OverallSuccess.IMultipleImplementationForTypeResolverFactory.generated.cs
#nullable enable
namespace OverallSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    public partial interface IMultipleImplementationForTypeResolverFactory : global::Sundew.Injection.IGeneratedFactory
    {
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        global::OverallSuccess.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverA();

        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        global::OverallSuccess.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverB();
    }
}
