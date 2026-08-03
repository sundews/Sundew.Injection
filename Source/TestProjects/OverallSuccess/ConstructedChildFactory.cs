namespace OverallSuccess;

using OverallSuccess.ChildFactory;
using OverallSuccess.OptionalInterface;
using Sundew.Injection;

public partial class ConstructedChildFactory : IConstructedChildFactory
{
    public static partial ConstructedChildFactory Constructor([DefaultValue(default)] ILifecycleParameters? lifecycleParameters);
}

public partial interface IConstructedChildFactory
{
    ConstructedChild Create(IResolveRootFactory resolveRootFactory, OptionalParameters optionalParameters);
}

