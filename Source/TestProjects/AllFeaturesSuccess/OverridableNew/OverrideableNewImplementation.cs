using AllFeaturesSuccess.InterfaceImplementationBindings;
using AllFeaturesSuccess.SingleInstancePerRequest;
using System;

namespace AllFeaturesSuccess.OverridableNew;

public class OverrideableNewImplementation : IPrint
{
    private readonly IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest;
    private readonly IInjectableByInterface injectableByInterface;

    public OverrideableNewImplementation(IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest, IInjectableByInterface injectableByInterface)
    {
        this.injectableSingleInstancePerRequest = injectableSingleInstancePerRequest;
        this.injectableByInterface = injectableByInterface;
    }
    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        this.injectableSingleInstancePerRequest.PrintMe(indent + 2);
        this.injectableByInterface.PrintMe(indent + 2);
    }
}