//HintName: DistinctSuccess.SelectedConstructor.RootFactory.generated.cs
#nullable enable
namespace DistinctSuccess.SelectedConstructor
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory("Create")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class RootFactory : global::Sundew.Injection.IGeneratedFactory
    {
        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public RootFactory()
        {
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public partial global::DistinctSuccess.SelectedConstructor.Root Create(int index)
        {
            return new global::DistinctSuccess.SelectedConstructor.Root(new global::DistinctSuccess.SelectedConstructor.Dependency(index));
        }
    }
}
