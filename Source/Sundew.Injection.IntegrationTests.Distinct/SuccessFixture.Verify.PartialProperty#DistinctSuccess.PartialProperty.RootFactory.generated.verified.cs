//HintName: DistinctSuccess.PartialProperty.RootFactory.generated.cs
#nullable enable
namespace DistinctSuccess.PartialProperty
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory("Root")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class RootFactory : global::Sundew.Injection.IGeneratedFactory
    {
        private readonly global::DistinctSuccess.PartialProperty.IRoot root;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public RootFactory()
        {
            this.root = new global::DistinctSuccess.PartialProperty.Root();
        }

        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public partial global::DistinctSuccess.PartialProperty.IRoot Root
        {
            get
            {
                return this.root;
            }
        }
    }
}
