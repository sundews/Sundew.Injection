namespace OverallSuccess.Operations;

using OverallSuccessDependency;

public interface IOperation : IIdentifiable
{
    int Execute();
}