namespace AllFeaturesSuccess;

using AllFeaturesSuccessDependency;

public interface IPrint : IIdentifiable
{
    void PrintMe(int indent);
}