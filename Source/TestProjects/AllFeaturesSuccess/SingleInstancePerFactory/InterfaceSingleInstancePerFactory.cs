namespace AllFeaturesSuccess.SingleInstancePerFactory;

using System;
using AllFeaturesSuccess.InterfaceSegregationBindings;
using AllFeaturesSuccess.MultipleImplementations;
using AllFeaturesSuccess.RequiredInterface;
using AllFeaturesSuccessDependency;

public class InterfaceSingleInstancePerFactory : IInterfaceSingleInstancePerFactory
{
    private readonly IInjectedSeparately injectedSeparately;
    private readonly IMultipleImplementationForArray[] formatters;
    private readonly IGeneratedOperationFactory operationFactory;
    private readonly IInterfaceSegregationOverridableNewB interfaceSegregationOverridableNewB;

    public InterfaceSingleInstancePerFactory(IInjectedSeparately injectedSeparately, IMultipleImplementationForArray[] formatters, IGeneratedOperationFactory operationFactory, IInterfaceSegregationOverridableNewB interfaceSegregationOverridableNewB)
    {
        this.injectedSeparately = injectedSeparately;
        this.formatters = formatters;
        this.operationFactory = operationFactory;
        this.interfaceSegregationOverridableNewB = interfaceSegregationOverridableNewB;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public void Start()
    {
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        FactoryLifetime.Disposed(this);
    }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        Console.WriteLine(new string(' ', indent + 2) + this.injectedSeparately.GetType().Name);
        foreach (var formatter in this.formatters)
        {
            formatter.PrintMe(indent + 2);
        }

        Console.WriteLine(new string(' ', indent + 2) + this.operationFactory.GetType().Name);
        this.interfaceSegregationOverridableNewB.PrintMe(indent + 2);
    }
}
