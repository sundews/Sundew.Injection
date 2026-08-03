//HintName: DistinctSuccess.Parameters.OptionalReferenceType.RootFactory.generated.cs
#nullable enable
namespace DistinctSuccess.Parameters.OptionalReferenceType
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
        public partial global::DistinctSuccess.Parameters.OptionalReferenceType.Root Create(global::DistinctSuccess.Parameters.OptionalReferenceType.IOption? option)
        {
            return new global::DistinctSuccess.Parameters.OptionalReferenceType.Root(option);
        }
    }
}
