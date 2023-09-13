//HintName: AllFeaturesSuccess.GeneratedOperationFactory.generated.cs
#nullable enable
namespace AllFeaturesSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class GeneratedOperationFactory : global::AllFeaturesSuccess.IGeneratedOperationFactory
    {
        public GeneratedOperationFactory()
        {
        }

        [global::Sundew.Injection.CreateMethod]
        public global::AllFeaturesSuccess.Operations.IOperation CreateOperationA(int lhs, int rhs)
        {
            return new global::AllFeaturesSuccess.Operations.OperationA(lhs, rhs);
        }

        [global::Sundew.Injection.CreateMethod]
        public global::AllFeaturesSuccess.Operations.IOperation CreateOperationB(int lhs, int rhs)
        {
            return new global::AllFeaturesSuccess.Operations.OperationB(lhs, rhs);
        }
    }
}
