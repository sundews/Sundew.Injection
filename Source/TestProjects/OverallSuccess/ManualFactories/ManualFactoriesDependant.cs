namespace OverallSuccess.ManualFactories;

using System;
using OverallSuccessDependency;

public class ManualFactoriesDependant : IPrint
{
    private readonly ManualSingletonDependency manualSingletonDependency;
    private readonly ManualDependency manualDependency;

    public ManualFactoriesDependant(ManualSingletonDependency manualSingletonDependency, ManualDependency manualDependency)
    {
        this.manualSingletonDependency = manualSingletonDependency;
        this.manualDependency = manualDependency;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        Console.WriteLine(new string(' ', indent + 2) + this.manualSingletonDependency.GetType().Name);
        Console.WriteLine(new string(' ', indent + 2) + this.manualDependency.GetType().Name);
    }
}