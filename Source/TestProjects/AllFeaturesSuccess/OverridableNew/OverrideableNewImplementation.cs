namespace AllFeaturesSuccess.OverridableNew;

using System;
using AllFeaturesSuccess.InterfaceImplementationBindings;
using AllFeaturesSuccess.RequiredInterface;
using AllFeaturesSuccess.SingleInstancePerRequest;

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
    }
    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        this.injectableSingleInstancePerRequest.PrintMe(indent + 2);
        this.injectableByInterface.PrintMe(indent + 2);
        this.requiredParameter.PrintMe(indent + 2);
    }
}