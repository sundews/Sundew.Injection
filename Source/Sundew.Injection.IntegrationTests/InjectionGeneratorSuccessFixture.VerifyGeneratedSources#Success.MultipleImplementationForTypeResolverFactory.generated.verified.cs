//HintName: Success.MultipleImplementationForTypeResolverFactory.generated.cs
#nullable enable
namespace Success
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class MultipleImplementationForTypeResolverFactory : global::Success.IMultipleImplementationForTypeResolverFactory
    {
        private readonly global::Success.TypeResolver.DependencyShared dependencyShared;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public MultipleImplementationForTypeResolverFactory()
        {
            this.dependencyShared = new global::Success.TypeResolver.DependencyShared();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        public global::Success.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverA()
        {
            return new global::Success.TypeResolver.MultipleImplementationForTypeResolverA(new global::Success.TypeResolver.DependencyA(this.dependencyShared));
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        public global::Success.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverB()
        {
            return new global::Success.TypeResolver.MultipleImplementationForTypeResolverB(new global::Success.TypeResolver.DependencyB(this.dependencyShared));
        }
    }
}
