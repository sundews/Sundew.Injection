namespace OverallSuccess;

public interface IResolveRoot : IPrint
{
    global::OverallSuccess.InterfaceImplementationBindings.IIntercepted Intercepted { get; }

    global::OverallSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory InterfaceSingleInstancePerFactory { get; }
}