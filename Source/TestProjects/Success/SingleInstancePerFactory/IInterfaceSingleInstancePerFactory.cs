namespace Success.SingleInstancePerFactory;

using System;
using Initialization.Interfaces;

public interface IInterfaceSingleInstancePerFactory : IAsyncInitializable, IDisposable, IPrint
{
    void Start();
}