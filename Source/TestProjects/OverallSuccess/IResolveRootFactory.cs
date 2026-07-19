namespace OverallSuccess;

using System.Collections.Generic;
using OverallSuccess.RequiredInterface;
using OverallSuccess.SingleInstancePerFactory;
using OverallSuccess.TypeResolver;

public partial interface IResolveRootFactory
{
    public IInterfaceSingleInstancePerFactory InterfaceSingleInstancePerFactory();

    public IResolveRoot CreateResolveRoot(
        IEnumerable<int> integers,
        int defaultItem,
        System.Func<IRequiredService> requiredService,
        RequiredParameter requiredParameter,
        SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
        InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null);

    public IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverC();
}