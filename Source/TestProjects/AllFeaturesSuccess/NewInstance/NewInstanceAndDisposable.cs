namespace AllFeaturesSuccess.NewInstance;

using System;
using AllFeaturesSuccess.OptionalInterface;

public class NewInstanceAndDisposable : IPrint, IDisposable
{
    private readonly IOmittedOptional? optionalImplementation;

    public NewInstanceAndDisposable(IOmittedOptional? optionalImplementation = null)
    {
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