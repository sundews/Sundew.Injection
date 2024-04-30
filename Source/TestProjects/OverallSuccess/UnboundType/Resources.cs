namespace OverallSuccess.UnboundType;

using System;
using OverallSuccess.NewInstance;
using OverallSuccessDependency;

public class Resources : IPrint
{
    private readonly NewInstanceAndDisposable newInstanceAndDisposable;

    public Resources(NewInstanceAndDisposable newInstanceAndDisposable)
    {
        this.newInstanceAndDisposable = newInstanceAndDisposable;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        this.newInstanceAndDisposable.PrintMe(indent + 2);
    }
}