namespace AllFeaturesSuccess.InterfaceSegregationBindings;

using AllFeaturesSuccess.RequiredInterface;
using System;

public sealed class InterfaceSegregationOverridableNewImplementation : IInterfaceSegregationOverridableNew, IDisposable
{
    private readonly IInjectedByType injectedByType;

    public InterfaceSegregationOverridableNewImplementation(IInjectedByType injectedByType)
    {
        this.injectedByType = injectedByType;
    }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        Console.WriteLine(new string(' ', indent) + this.injectedByType.GetType().Name);
    }

    public void Dispose()
    {
    }
}