//HintName: GeneratedOperationFactory.cs
namespace AllFeaturesSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.0.0.0")]
    [global::Sundew.Injection.Factory]
    public sealed class GeneratedOperationFactory : global::AllFeaturesSuccess.IGeneratedOperationFactory
    {
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public GeneratedOperationFactory()
        {
        }

        [global::Sundew.Injection.CreateMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public global::AllFeaturesSuccess.Operations.IOperation CreateOperationA(int lhs, int rhs)
        {
            return new global::AllFeaturesSuccess.Operations.OperationA(lhs, rhs);
        }

        [global::Sundew.Injection.CreateMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public global::AllFeaturesSuccess.Operations.IOperation CreateOperationB(int lhs, int rhs)
        {
            return new global::AllFeaturesSuccess.Operations.OperationB(lhs, rhs);
        }
    }
}
