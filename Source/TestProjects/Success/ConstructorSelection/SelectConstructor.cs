namespace Success.ConstructorSelection;

using System;
using System.Threading.Tasks;
using Success.InterfaceSegregationBindings;
using Success.SingleInstancePerFactory;
using Success.SingleInstancePerRequest;
using SuccessDependency;

public class SelectConstructor : ISelectConstructor
{
    private readonly ImplementationSingleInstancePerFactory implementationSingleInstancePerFactory;
    private readonly IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest;
    private readonly IInterfaceSegregationA interfaceSegregationA;

    public SelectConstructor(ImplementationSingleInstancePerFactory implementationSingleInstancePerFactory, IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest, IInterfaceSegregationA interfaceSegregationA)
    {
        this.implementationSingleInstancePerFactory = implementationSingleInstancePerFactory;
        this.injectableSingleInstancePerRequest = injectableSingleInstancePerRequest;
        this.interfaceSegregationA = interfaceSegregationA;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        FactoryLifetime.Disposed(this);
        return default;
    }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        this.implementationSingleInstancePerFactory.PrintMe(indent + 2);
        this.injectableSingleInstancePerRequest.PrintMe(indent + 2);
        this.interfaceSegregationA.PrintMe(indent + 2);
    }
}