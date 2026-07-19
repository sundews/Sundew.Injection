namespace OverallSuccess;

using OverallSuccess.Operations;

public partial class GeneratedOperationFactory : IGeneratedOperationFactory
{
}

public partial interface IGeneratedOperationFactory
{
    public IOperation CreateOperationA(int lhs, int rhs);

    public IOperation CreateOperationB(int lhs, int rhs);
}