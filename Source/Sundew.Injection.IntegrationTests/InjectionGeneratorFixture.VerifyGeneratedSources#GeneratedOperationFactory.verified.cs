//HintName: GeneratedOperationFactory.cs
namespace AllFeaturesSuccess
{
    public sealed class GeneratedOperationFactory : global::AllFeaturesSuccess.IGeneratedOperationFactory
    {
        public GeneratedOperationFactory()
        {
        }

        public global::AllFeaturesSuccess.Operations.IOperation CreateOperationA(int lhs, int rhs)
        {
            return new global::AllFeaturesSuccess.Operations.OperationA(lhs, rhs);
        }

        public global::AllFeaturesSuccess.Operations.IOperation CreateOperationB(int lhs, int rhs)
        {
            return new global::AllFeaturesSuccess.Operations.OperationB(lhs, rhs);
        }
    }
}
