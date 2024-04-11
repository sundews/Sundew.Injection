namespace Success.SingleInstancePerFactory;

using System;

public interface IInterfaceSingleInstancePerFactory : IDisposable, IPrint
{
    void Start();
}