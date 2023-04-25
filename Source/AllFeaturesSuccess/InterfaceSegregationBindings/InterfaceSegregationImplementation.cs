namespace AllFeaturesSuccess.InterfaceSegregationBindings;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AllFeaturesSuccess.SingleInstancePerRequest;

public sealed class InterfaceSegregationImplementation : IDisposable, IInterfaceSegregation
{
    private readonly Dictionary<string, string> registry = new();

    public InterfaceSegregationImplementation(IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest)
    {
    }

    public bool TryGet(string key, [NotNullWhen(true)] out string? value)
    {
        return this.registry.TryGetValue(key, out value);
    }

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
    }
}