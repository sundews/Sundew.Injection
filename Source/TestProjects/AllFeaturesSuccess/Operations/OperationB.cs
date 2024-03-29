namespace AllFeaturesSuccess.Operations;

public class OperationB : IOperation
{
    private readonly int lhs;
    private readonly int rhs;

    public OperationB(int lhs, int rhs)
    {
        this.lhs = lhs;
        this.rhs = rhs;
    }

    public int Execute()
    {
        return this.lhs - this.rhs;
    }
}