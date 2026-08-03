//HintName: DistinctSuccess.Parameters.OptionalIntToRequired.RootFactory.generated.cs
#nullable enable
namespace DistinctSuccess.Parameters.OptionalIntToRequired
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
        public partial global::DistinctSuccess.Parameters.OptionalIntToRequired.Root Create(int? index)
        {
            return new global::DistinctSuccess.Parameters.OptionalIntToRequired.Root(index.GetValueOrDefault());
        }
    }
}
