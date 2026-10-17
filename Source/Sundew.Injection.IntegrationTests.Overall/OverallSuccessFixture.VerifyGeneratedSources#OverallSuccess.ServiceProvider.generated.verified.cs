//HintName: OverallSuccess.ServiceProvider.generated.cs
#nullable enable
namespace OverallSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class ServiceProvider : global::Sundew.Injection.IInjectionServiceProvider
    {
        private const int BucketSize = 41;
        private readonly global::OverallSuccess.SundewInjection.ServiceResolver serviceResolver;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public ServiceProvider(
            global::OverallSuccess.MultipleImplementationForTypeResolverFactory multipleImplementationForTypeResolverFactory,
            global::OverallSuccessDependency.DependencyFactory dependencyFactory,
            global::OverallSuccessDependency.ManualMultipleDependencyFactory manualMultipleDependencyFactory,
            global::OverallSuccess.GeneratedOperationFactory generatedOperationFactory,
            global::OverallSuccess.ResolveRootFactory resolveRootFactory)
        {
            this.serviceResolver = new global::OverallSuccess.SundewInjection.ServiceResolver(
                BucketSize,
                new global::OverallSuccess.SundewInjection.ResolverItem(typeof(global::OverallSuccess.TypeResolver.IMultipleImplementationForTypeResolver), () => new object[] { multipleImplementationForTypeResolverFactory.CreateMultipleImplementationForTypeResolverA(), multipleImplementationForTypeResolverFactory.CreateMultipleImplementationForTypeResolverB(), resolveRootFactory.CreateMultipleImplementationForTypeResolverC() }),
                new global::OverallSuccess.SundewInjection.ResolverItem(typeof(global::OverallSuccessDependency.Dependency), () => dependencyFactory.Create()),
                new global::OverallSuccess.SundewInjection.ResolverItem(
                    typeof(global::OverallSuccessDependency.ManualMultipleDependency),
                    () => manualMultipleDependencyFactory.CreateNewInstance(),
                    (instance) =>
                    {
                        if (instance is global::OverallSuccessDependency.ManualMultipleDependency actualInstance)
                        {
                            manualMultipleDependencyFactory.Dispose(actualInstance);
                            return true;
                        }

                        return false;
                    },
                    (instance) =>
                    {
                        if (instance is global::OverallSuccessDependency.ManualMultipleDependency actualInstance)
                        {
                            manualMultipleDependencyFactory.Dispose(actualInstance);
                            return new global::System.Threading.Tasks.ValueTask<bool>(true);
                        }

                        return new global::System.Threading.Tasks.ValueTask<bool>(false);
                    }),
                new global::OverallSuccess.SundewInjection.ResolverItem(
                    typeof(global::OverallSuccessDependency.ManualMultipleSingletonDependency),
                    () => manualMultipleDependencyFactory.ManualMultipleSingletonDependency,
                    (instance) =>
                    {
                        if (instance is global::OverallSuccessDependency.ManualMultipleSingletonDependency actualInstance)
                        {
                            manualMultipleDependencyFactory.DisposeAsync(actualInstance).AsTask().GetAwaiter().GetResult();
                            return true;
                        }

                        return false;
                    },
                    async (instance) =>
                    {
                        if (instance is global::OverallSuccessDependency.ManualMultipleSingletonDependency actualInstance)
                        {
                            await manualMultipleDependencyFactory.DisposeAsync(actualInstance).ConfigureAwait(false);
                            return true;
                        }

                        return false;
                    }),
                new global::OverallSuccess.SundewInjection.ResolverItem(typeof(global::OverallSuccess.Operations.IOperation), null, new global::OverallSuccess.SundewInjection.ArgumentResolver(new global::System.Type[] { typeof(int), typeof(int) }, (arguments) => generatedOperationFactory.CreateOperationA((int)arguments[0]!, (int)arguments[1]!))),
                new global::OverallSuccess.SundewInjection.ResolverItem(typeof(global::OverallSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory), () => resolveRootFactory.InterfaceSingleInstancePerFactory()),
                new global::OverallSuccess.SundewInjection.ResolverItem(
                    typeof(global::OverallSuccess.IResolveRoot),
                    null,
                    (instance) =>
                    {
                        if (instance is global::OverallSuccess.IResolveRoot actualInstance)
                        {
                            resolveRootFactory.Dispose(actualInstance);
                            return true;
                        }

                        return false;
                    },
                    async (instance) =>
                    {
                        if (instance is global::OverallSuccess.IResolveRoot actualInstance)
                        {
                            await resolveRootFactory.DisposeAsync(actualInstance).ConfigureAwait(false);
                            return true;
                        }

                        return false;
                    },
                    new global::OverallSuccess.SundewInjection.ArgumentResolver(new global::System.Type[] { 
                            typeof(global::System.Collections.Generic.IEnumerable<int>),
                            typeof(int),
                            typeof(global::System.Func<global::OverallSuccess.RequiredInterface.IRequiredService>),
                            typeof(global::OverallSuccess.RequiredInterface.RequiredParameter) }, (arguments) => resolveRootFactory.CreateResolveRoot(
                            (global::System.Collections.Generic.IEnumerable<int>)arguments[0]!,
                            (int)arguments[1]!,
                            (global::System.Func<global::OverallSuccess.RequiredInterface.IRequiredService>)arguments[2]!,
                            (global::OverallSuccess.RequiredInterface.RequiredParameter)arguments[3]!)),
                    new global::OverallSuccess.SundewInjection.ArgumentResolver(new global::System.Type[] { 
                            typeof(global::System.Collections.Generic.IEnumerable<int>),
                            typeof(int),
                            typeof(global::System.Func<global::OverallSuccess.RequiredInterface.IRequiredService>),
                            typeof(global::OverallSuccess.RequiredInterface.RequiredParameter),
                            typeof(global::OverallSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest) }, (arguments) => resolveRootFactory.CreateResolveRoot(
                            (global::System.Collections.Generic.IEnumerable<int>)arguments[0]!,
                            (int)arguments[1]!,
                            (global::System.Func<global::OverallSuccess.RequiredInterface.IRequiredService>)arguments[2]!,
                            (global::OverallSuccess.RequiredInterface.RequiredParameter)arguments[3]!,
                            (global::OverallSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest)arguments[4]!)),
                    new global::OverallSuccess.SundewInjection.ArgumentResolver(new global::System.Type[] { 
                            typeof(global::System.Collections.Generic.IEnumerable<int>),
                            typeof(int),
                            typeof(global::System.Func<global::OverallSuccess.RequiredInterface.IRequiredService>),
                            typeof(global::OverallSuccess.RequiredInterface.RequiredParameter),
                            typeof(global::OverallSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest),
                            typeof(global::OverallSuccess.InterfaceSegregationBindings.IInterfaceSegregation) }, (arguments) => resolveRootFactory.CreateResolveRoot(
                            (global::System.Collections.Generic.IEnumerable<int>)arguments[0]!,
                            (int)arguments[1]!,
                            (global::System.Func<global::OverallSuccess.RequiredInterface.IRequiredService>)arguments[2]!,
                            (global::OverallSuccess.RequiredInterface.RequiredParameter)arguments[3]!,
                            (global::OverallSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest)arguments[4]!,
                            (global::OverallSuccess.InterfaceSegregationBindings.IInterfaceSegregation)arguments[5]!))));
        }

        public object? GetService(global::System.Type serviceType)
        {
            return this.serviceResolver.TryGetService(serviceType, out var service, default) ? service : null;
        }

        public bool TryGetService(global::System.Type serviceType, out object? service, params global::System.ReadOnlySpan<object?> arguments)
        {
            return this.serviceResolver.TryGetService(serviceType, out service, arguments);
        }

        public bool TryDispose(object instance)
        {
            return this.serviceResolver.TryDispose(instance);
        }

        public global::System.Threading.Tasks.ValueTask<bool> TryDisposeAsync(object instance)
        {
            return this.serviceResolver.TryDisposeAsync(instance);
        }
    }
}
