//HintName: DistinctSuccess.PartialConstructor.RootFactory.generated.cs
#nullable enable
namespace DistinctSuccess.PartialConstructor
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory("Create")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class RootFactory : global::Sundew.Injection.IGeneratedFactory
    {
        private readonly global::DistinctSuccess.PartialConstructor.Dependency dependency;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public partial RootFactory(global::DistinctSuccess.PartialConstructor.Dependency dependency)
        {
            this.dependency = dependency;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public partial global::DistinctSuccess.PartialConstructor.Root Create()
        {
            return new global::DistinctSuccess.PartialConstructor.Root(this.dependency);
        }
    }
}
