//HintName: OverallSuccess.GeneratedOperationFactory.generated.cs
#nullable enable
namespace OverallSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class GeneratedOperationFactory : global::OverallSuccess.IGeneratedOperationFactory
    {
        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public GeneratedOperationFactory()
        {
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public global::OverallSuccess.Operations.IOperation CreateOperationA(int lhs, int rhs)
        {
            return new global::OverallSuccess.Operations.OperationA(lhs, rhs);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public global::OverallSuccess.Operations.IOperation CreateOperationB(int lhs, int rhs)
        {
            return new global::OverallSuccess.Operations.OperationB(lhs, rhs);
        }
    }
}
