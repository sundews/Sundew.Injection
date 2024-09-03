//HintName: DistinctSuccess.DependencyFromBoundInterfaceFactory.RootFactory.generated.cs
#nullable enable
namespace DistinctSuccess.DependencyFromBoundInterfaceFactory
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class RootFactory : global::Sundew.Injection.IGeneratedFactory
    {
        private readonly global::DistinctSuccess.DependencyFromBoundInterfaceFactory.IFactory factory;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public RootFactory()
        {
            this.factory = new global::DistinctSuccess.DependencyFromBoundInterfaceFactory.Factory();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public global::DistinctSuccess.DependencyFromBoundInterfaceFactory.Root Create()
        {
            return new global::DistinctSuccess.DependencyFromBoundInterfaceFactory.Root(this.factory.Create());
        }
    }
}
