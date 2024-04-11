//HintName: Success.GeneratedOperationFactory.generated.cs
#nullable enable
namespace Success
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class GeneratedOperationFactory : global::Success.IGeneratedOperationFactory
    {
        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public GeneratedOperationFactory()
        {
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        public global::Success.Operations.IOperation CreateOperationA(int lhs, int rhs)
        {
            return new global::Success.Operations.OperationA(lhs, rhs);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        public global::Success.Operations.IOperation CreateOperationB(int lhs, int rhs)
        {
            return new global::Success.Operations.OperationB(lhs, rhs);
        }
    }
}
