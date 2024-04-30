//HintName: OverallSuccess.MultipleImplementationForTypeResolverFactory.generated.cs
#nullable enable
namespace OverallSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class MultipleImplementationForTypeResolverFactory : global::OverallSuccess.IMultipleImplementationForTypeResolverFactory
    {
        private readonly global::OverallSuccess.TypeResolver.DependencyShared dependencyShared;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public MultipleImplementationForTypeResolverFactory()
        {
            this.dependencyShared = new global::OverallSuccess.TypeResolver.DependencyShared();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public global::OverallSuccess.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverA()
        {
            return new global::OverallSuccess.TypeResolver.MultipleImplementationForTypeResolverA(new global::OverallSuccess.TypeResolver.DependencyA(this.dependencyShared));
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public global::OverallSuccess.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverB()
        {
            return new global::OverallSuccess.TypeResolver.MultipleImplementationForTypeResolverB(new global::OverallSuccess.TypeResolver.DependencyB(this.dependencyShared));
        }
    }
}
