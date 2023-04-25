namespace AllFeaturesSuccess.Operations
{
    public class OperationA : IOperation
    {
        private readonly int lhs;
        private readonly int rhs;

        public OperationA(int lhs, int rhs)
        {
            this.lhs = lhs;
            this.rhs = rhs;
        }

        public int Execute()
        {
            return this.lhs + this.rhs;
        }
    }
}