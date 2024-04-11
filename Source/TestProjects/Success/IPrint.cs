namespace Success;

using SuccessDependency;

public interface IPrint : IIdentifiable
{
    void PrintMe(int indent);
}