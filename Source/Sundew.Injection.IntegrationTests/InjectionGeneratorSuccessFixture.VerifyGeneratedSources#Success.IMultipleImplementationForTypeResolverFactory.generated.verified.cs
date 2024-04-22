//HintName: Success.IMultipleImplementationForTypeResolverFactory.generated.cs
#nullable enable
namespace Success
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    public partial interface IMultipleImplementationForTypeResolverFactory : global::Sundew.Injection.IGeneratedFactory
    {
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        global::Success.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverA();

        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        global::Success.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverB();
    }
}
