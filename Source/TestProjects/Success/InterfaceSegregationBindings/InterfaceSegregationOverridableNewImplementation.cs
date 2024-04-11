namespace Success.InterfaceSegregationBindings;

using System;
using Success.RequiredInterface;
using SuccessDependency;

public sealed class InterfaceSegregationOverridableNewImplementation : IInterfaceSegregationOverridableNew, IDisposable
{
    private readonly IInjectedByType injectedByType;

    public InterfaceSegregationOverridableNewImplementation(IInjectedByType injectedByType)
    {
        this.injectedByType = injectedByType;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        Console.WriteLine(new string(' ', indent) + this.injectedByType.GetType().Name);
    }

    public void Dispose()
    {
        FactoryLifetime.Disposed(this);
    }
}