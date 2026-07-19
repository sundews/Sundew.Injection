namespace OverallSuccess;

using OverallSuccess.OptionalInterface;
using OverallSuccess.RequiredInterface;
using Sundew.Injection;

public partial class ResolveRootFactory : IResolveRootFactory
{
    public static partial ResolveRootFactory Constructor(
        IRequiredParameters requiredParameters,
        IInjectedSeparately injectedSeparatelyForInterfaceSingleInstancePerFactory,
        IInjectedByType injectedByType,
        IInjectedSeparately injectedSeparatelyForImplementationSingleInstancePerFactory,
        OptionalParameters optionalParameters,
        string name,
        [DefaultValue(default)] ILifecycleParameters? lifecycleParameters);
}