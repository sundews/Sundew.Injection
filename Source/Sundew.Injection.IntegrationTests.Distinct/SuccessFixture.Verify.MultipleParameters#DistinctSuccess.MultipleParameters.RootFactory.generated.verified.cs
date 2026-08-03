//HintName: DistinctSuccess.MultipleParameters.RootFactory.generated.cs
#nullable enable
namespace DistinctSuccess.MultipleParameters
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory("Create")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class RootFactory : global::Sundew.Injection.IGeneratedFactory
    {
        private readonly global::DistinctSuccess.MultipleParameters.IRequired1 required1;
        private readonly global::DistinctSuccess.MultipleParameters.IRequired2 required2;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public RootFactory(global::DistinctSuccess.MultipleParameters.IRequired1 required1, global::DistinctSuccess.MultipleParameters.IRequired2 required2)
        {
            this.required1 = required1;
            this.required2 = required2;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public partial global::DistinctSuccess.MultipleParameters.Root Create()
        {
            return new global::DistinctSuccess.MultipleParameters.Root(this.required1, this.required2);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        private static partial global::DistinctSuccess.MultipleParameters.RootFactory Constructor(global::DistinctSuccess.MultipleParameters.IRequired1 required1, global::DistinctSuccess.MultipleParameters.IRequired2 required2)
        {
            return new global::DistinctSuccess.MultipleParameters.RootFactory(required1, required2);
        }
    }
}
