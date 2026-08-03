//HintName: DistinctSuccess.Parameters.ConstructorOptionalReferenceType.RootFactory.generated.cs
#nullable enable
namespace DistinctSuccess.Parameters.ConstructorOptionalReferenceType
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory("Create")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class RootFactory : global::Sundew.Injection.IGeneratedFactory
    {
        private readonly global::DistinctSuccess.Parameters.ConstructorOptionalReferenceType.IOption? option;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public RootFactory(global::DistinctSuccess.Parameters.ConstructorOptionalReferenceType.IOption? option = default)
        {
            this.option = option;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public partial global::DistinctSuccess.Parameters.ConstructorOptionalReferenceType.Root Create()
        {
            return new global::DistinctSuccess.Parameters.ConstructorOptionalReferenceType.Root(this.option);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public static partial global::DistinctSuccess.Parameters.ConstructorOptionalReferenceType.RootFactory Constructor(global::DistinctSuccess.Parameters.ConstructorOptionalReferenceType.IOption? option)
        {
            return new global::DistinctSuccess.Parameters.ConstructorOptionalReferenceType.RootFactory(option);
        }
    }
}
