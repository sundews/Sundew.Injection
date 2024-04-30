namespace OverallSuccess.OverridableNew;

using System;
using OverallSuccess.InterfaceImplementationBindings;
using OverallSuccess.RequiredInterface;
using OverallSuccess.SingleInstancePerRequest;
using OverallSuccessDependency;

public class OverrideableNewImplementation : IPrint
{
    private readonly IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest;
    private readonly IInjectableByInterface injectableByInterface;
    private readonly RequiredParameter requiredParameter;

    public OverrideableNewImplementation(IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest, IInjectableByInterface injectableByInterface, RequiredParameter requiredParameter)
    {
        this.injectableSingleInstancePerRequest = injectableSingleInstancePerRequest;
        this.injectableByInterface = injectableByInterface;
        this.requiredParameter = requiredParameter;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        this.injectableSingleInstancePerRequest.PrintMe(indent + 2);
        this.injectableByInterface.PrintMe(indent + 2);
        this.requiredParameter.PrintMe(indent + 2);
    }
}