namespace OverallSuccess.InterfaceSegregationBindings;

using System;
using System.Collections.Generic;
using OverallSuccess.SingleInstancePerRequest;
using OverallSuccessDependency;

public sealed class InterfaceSegregationImplementation : IDisposable, IInterfaceSegregation
{
    private readonly Dictionary<string, string> registry = new();

    public InterfaceSegregationImplementation(IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest)
    {
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public IInterfaceSegregationA Add(string key, string value)
    {
        this.registry[key] = value;
        return this;
    }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
    }

    public void Dispose()
    {
        FactoryLifetime.Disposed(this);
    }
}