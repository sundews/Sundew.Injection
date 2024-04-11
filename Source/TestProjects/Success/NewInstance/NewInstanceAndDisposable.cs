namespace Success.NewInstance;

using System;
using Success.OptionalInterface;
using SuccessDependency;

public class NewInstanceAndDisposable : IPrint, IDisposable
{
    private readonly IResolveRootFactory resolveRootFactory;
    private readonly IOmittedOptional? optionalImplementation;

    public NewInstanceAndDisposable(IResolveRootFactory resolveRootFactory, IOmittedOptional? optionalImplementation = null)
    {
        this.resolveRootFactory = resolveRootFactory;
        this.optionalImplementation = optionalImplementation;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        FactoryLifetime.Disposed(this);
    }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        this.optionalImplementation?.PrintMe(indent + 2);
    }
}