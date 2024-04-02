namespace AllFeaturesSuccess.ConstructorSelection;

using System;
using AllFeaturesSuccess.InterfaceSegregationBindings;
using AllFeaturesSuccess.RequiredInterface;
using AllFeaturesSuccess.SingleInstancePerFactory;
using AllFeaturesSuccess.SingleInstancePerRequest;
using AllFeaturesSuccessDependency;

public class SelectFactoryMethod : ISelectFactoryMethod
{
    private readonly IInterfaceSingleInstancePerFactory interfaceSingleInstancePerFactory;
    private readonly IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest;
    private readonly IRequiredService requiredService;
    private readonly IInterfaceSegregationB interfaceSegregationB;

    public SelectFactoryMethod(IInterfaceSingleInstancePerFactory interfaceSingleInstancePerFactory, IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest, IRequiredService requiredService, IInterfaceSegregationB interfaceSegregationB)
    {
        this.interfaceSingleInstancePerFactory = interfaceSingleInstancePerFactory;
        this.injectableSingleInstancePerRequest = injectableSingleInstancePerRequest;
        this.requiredService = requiredService;
        this.interfaceSegregationB = interfaceSegregationB;
        this.interfaceSingleInstancePerFactory.Start();
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        Console.WriteLine(new string(' ', indent + 2) + this.requiredService.GetType().Name);
        this.interfaceSingleInstancePerFactory.PrintMe(indent + 2);
        this.interfaceSegregationB.PrintMe(indent + 2);
        this.injectableSingleInstancePerRequest.PrintMe(indent + 2);
    }
}