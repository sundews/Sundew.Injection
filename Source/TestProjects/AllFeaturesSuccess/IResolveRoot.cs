namespace AllFeaturesSuccess;

public interface IResolveRoot : IPrint
{
    global::AllFeaturesSuccess.InterfaceImplementationBindings.IIntercepted Intercepted { get; }

    global::AllFeaturesSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory InterfaceSingleInstancePerFactory { get; }
}