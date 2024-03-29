namespace AllFeaturesSuccess.InterfaceImplementationBindings;

public interface IIntercepted : IPrint
{
    public string Title { get; }

    public string Description { get; }
}