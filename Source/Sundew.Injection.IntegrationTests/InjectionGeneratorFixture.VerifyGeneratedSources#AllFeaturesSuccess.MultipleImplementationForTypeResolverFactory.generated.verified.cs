//HintName: AllFeaturesSuccess.MultipleImplementationForTypeResolverFactory.generated.cs
#nullable enable
namespace AllFeaturesSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class MultipleImplementationForTypeResolverFactory : global::AllFeaturesSuccess.IMultipleImplementationForTypeResolverFactory
    {
        private readonly global::AllFeaturesSuccess.TypeResolver.DependencyShared dependencyShared;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public MultipleImplementationForTypeResolverFactory()
        {
            this.dependencyShared = new global::AllFeaturesSuccess.TypeResolver.DependencyShared();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        public global::AllFeaturesSuccess.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverA()
        {
            return new global::AllFeaturesSuccess.TypeResolver.MultipleImplementationForTypeResolverA(new global::AllFeaturesSuccess.TypeResolver.DependencyA(this.dependencyShared));
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        public global::AllFeaturesSuccess.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverB()
        {
            return new global::AllFeaturesSuccess.TypeResolver.MultipleImplementationForTypeResolverB(new global::AllFeaturesSuccess.TypeResolver.DependencyB(this.dependencyShared));
        }
    }
}
