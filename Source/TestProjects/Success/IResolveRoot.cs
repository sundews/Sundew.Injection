namespace Success;

public interface IResolveRoot : IPrint
{
    global::Success.InterfaceImplementationBindings.IIntercepted Intercepted { get; }

    global::Success.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory InterfaceSingleInstancePerFactory { get; }
}