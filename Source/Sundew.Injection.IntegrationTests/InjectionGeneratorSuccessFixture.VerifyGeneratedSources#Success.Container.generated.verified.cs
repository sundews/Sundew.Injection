//HintName: Success.Container.generated.cs
#nullable enable
namespace Success
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class Container : global::System.IServiceProvider
    {
        private const int BucketSize = 13;
        private readonly global::Sundew.Injection.ResolverItem[] resolverItems;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public Container(
            global::Success.MultipleImplementationForTypeResolverFactory multipleImplementationForTypeResolverFactory,
            global::SuccessDependency.DependencyFactory dependencyFactory,
            global::SuccessDependency.ManualDependencyFactory manualDependencyFactory,
            global::Success.GeneratedOperationFactory generatedOperationFactory,
            global::Success.ResolveRootFactory resolveRootFactory)
        {
            this.resolverItems = global::Sundew.Injection.ResolverItemsFactory.Create(
                BucketSize,
                new global::Sundew.Injection.ResolverItem(typeof(global::Success.TypeResolver.IMultipleImplementationForTypeResolver), () => new object[] { multipleImplementationForTypeResolverFactory.CreateMultipleImplementationForTypeResolverA(), multipleImplementationForTypeResolverFactory.CreateMultipleImplementationForTypeResolverB(), resolveRootFactory.CreateMultipleImplementationForTypeResolverC() }),
                new global::Sundew.Injection.ResolverItem(typeof(global::SuccessDependency.Dependency), () => dependencyFactory.Create()),
                new global::Sundew.Injection.ResolverItem(typeof(global::SuccessDependency.ManualDependency), () => manualDependencyFactory.Create()),
                new global::Sundew.Injection.ResolverItem(typeof(global::Success.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory), () => resolveRootFactory.CreateInterfaceSingleInstancePerFactory()));
        }

        public object GetService(global::System.Type serviceType)
        {
            var index = global::System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(serviceType) % BucketSize;
            do
            {
                ref var item = ref this.resolverItems[index];
                if (ReferenceEquals(item.Type, serviceType))
                {
                    return item.Resolve.Invoke();
                }
            }
            while (index++ < BucketSize);

            throw new global::System.NotSupportedException($"The type: {serviceType} could not be found.");
        }                           
    }
}
