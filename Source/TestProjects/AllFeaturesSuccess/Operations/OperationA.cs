namespace AllFeaturesSuccess.Operations;

using AllFeaturesSuccessDependency;

public class OperationA : IOperation
{
    private readonly int lhs;
    private readonly int rhs;

    public OperationA(int lhs, int rhs)
    {
        this.lhs = lhs;
        this.rhs = rhs;
        this.Id = FactoryLifetime.Created((IIdentifiable)this);
    }

    public int Id { get; }

    public int Execute()
    {
        return this.lhs + this.rhs;
    }
}