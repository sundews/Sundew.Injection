namespace AllFeaturesSuccess.InterfaceSegregationBindings;

public interface IInterfaceSegregationA : IPrint
{
    IInterfaceSegregationA Add(string key, string value);
}