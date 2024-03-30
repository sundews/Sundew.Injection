namespace AllFeaturesSuccess.NewInstance;

using System;
using AllFeaturesSuccess.OptionalInterface;

public class NewInstanceAndDisposable : IPrint, IDisposable
{
    private readonly IResolveRootFactory resolveRootFactory;
    private readonly IOmittedOptional? optionalImplementation;

    public NewInstanceAndDisposable(IResolveRootFactory resolveRootFactory, IOmittedOptional? optionalImplementation = null)
    {
        this.resolveRootFactory = resolveRootFactory;
        this.optionalImplementation = optionalImplementation;
    }

    public void Dispose()
    {
    }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        this.optionalImplementation?.PrintMe(indent + 2);
    }
}