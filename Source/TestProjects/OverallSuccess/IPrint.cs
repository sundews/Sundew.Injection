namespace OverallSuccess;

using OverallSuccessDependency;

public interface IPrint : IIdentifiable
{
    void PrintMe(int indent);
}