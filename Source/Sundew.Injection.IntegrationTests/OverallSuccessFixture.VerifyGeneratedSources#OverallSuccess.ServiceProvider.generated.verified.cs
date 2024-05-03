//HintName: OverallSuccess.ServiceProvider.generated.cs
#nullable enable
namespace OverallSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class ServiceProvider : global::System.IServiceProvider
    {
        private const int BucketSize = 13;
        private readonly global::OverallSuccess.SundewInjection.ResolverItem[] resolverItems;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public ServiceProvider(
            global::OverallSuccess.MultipleImplementationForTypeResolverFactory multipleImplementationForTypeResolverFactory,
            global::OverallSuccessDependency.DependencyFactory dependencyFactory,
            global::OverallSuccessDependency.ManualMultipleDependencyFactory manualMultipleDependencyFactory,
            global::OverallSuccess.GeneratedOperationFactory generatedOperationFactory,
            global::OverallSuccess.ResolveRootFactory resolveRootFactory)
        {
            this.resolverItems = global::OverallSuccess.SundewInjection.ResolverItemsFactory.Create(
                BucketSize,
                new global::OverallSuccess.SundewInjection.ResolverItem(typeof(global::OverallSuccess.TypeResolver.IMultipleImplementationForTypeResolver), () => new object[] { multipleImplementationForTypeResolverFactory.CreateMultipleImplementationForTypeResolverA(), multipleImplementationForTypeResolverFactory.CreateMultipleImplementationForTypeResolverB(), resolveRootFactory.CreateMultipleImplementationForTypeResolverC() }),
                new global::OverallSuccess.SundewInjection.ResolverItem(typeof(global::OverallSuccessDependency.ManualMultipleSingletonDependency), () => manualMultipleDependencyFactory.ManualMultipleSingletonDependency),
                new global::OverallSuccess.SundewInjection.ResolverItem(typeof(global::OverallSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory), () => resolveRootFactory.InterfaceSingleInstancePerFactory));
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
