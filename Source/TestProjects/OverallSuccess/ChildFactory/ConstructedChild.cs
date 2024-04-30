namespace OverallSuccess.ChildFactory;

using System;
using System.Threading.Tasks;
using Initialization.Interfaces;
using OverallSuccess.NewInstance;
using OverallSuccessDependency;

public class ConstructedChild : IAsyncInitializable, IPrint
{
    private readonly NewInstanceAndDisposable newInstanceAndDisposable;
    private readonly Dependency dependency;
    private readonly ManualMultipleDependency manualMultipleDependency;

    public ConstructedChild(NewInstanceAndDisposable newInstanceAndDisposable, Dependency dependency, ManualMultipleDependency manualMultipleDependency)
    {
        this.newInstanceAndDisposable = newInstanceAndDisposable;
        this.dependency = dependency;
        this.manualMultipleDependency = manualMultipleDependency;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public ValueTask InitializeAsync()
    {
        FactoryLifetime.Initialized(this);
        return default;
    }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        this.newInstanceAndDisposable.PrintMe(indent + 2);
        Console.WriteLine(new string(' ', indent + 2) + this.dependency.GetType().Name);
        Console.WriteLine(new string(' ', indent + 2) + this.manualMultipleDependency.GetType().Name);
    }
}