//HintName: DistinctSuccess.DependencyFromBoundFactoryWithDisposable.MainFactory.generated.cs
#nullable enable
namespace DistinctSuccess.DependencyFromBoundFactoryWithDisposable
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory("Create")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class MainFactory : global::System.IDisposable, global::System.IAsyncDisposable, global::Sundew.Injection.IGeneratedFactory
    {
        private readonly global::Sundew.Injection.ILifecycleParameters lifecycleParameters;
        private readonly global::DependencyFromBoundFactoryWithDisposable.SundewInjection.LifecycleHandler lifecycleHandler;
        private readonly global::DistinctSuccess.DependencyFromBoundFactoryWithDisposable.DependencyFactory dependencyFactory;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public MainFactory()
        {
            this.lifecycleParameters = new global::Sundew.Injection.LifecycleParameters(
                false,
                false,
                default(global::Initialization.Interfaces.IInitializationReporter),
                default(global::Disposal.Interfaces.IDisposalReporter));
            this.lifecycleHandler = new global::DependencyFromBoundFactoryWithDisposable.SundewInjection.LifecycleHandler(this.lifecycleParameters, this.lifecycleParameters);
            this.dependencyFactory = new global::DistinctSuccess.DependencyFromBoundFactoryWithDisposable.DependencyFactory();
            this.lifecycleHandler.TryAdd(this.dependencyFactory);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public partial global::DistinctSuccess.DependencyFromBoundFactoryWithDisposable.Root Create()
        {
            var childLifecycleHandler = this.lifecycleHandler.CreateChildLifecycleHandler();
            var rootResult = new global::DistinctSuccess.DependencyFromBoundFactoryWithDisposable.Root(this.dependencyFactory.Dependency);
            this.lifecycleHandler.TryAdd(rootResult, childLifecycleHandler);
            return rootResult;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose(global::DistinctSuccess.DependencyFromBoundFactoryWithDisposable.Root root)
        {
            this.lifecycleHandler.Dispose(root);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::System.Threading.Tasks.ValueTask DisposeAsync(global::DistinctSuccess.DependencyFromBoundFactoryWithDisposable.Root root)
        {
            return this.lifecycleHandler.DisposeAsync(root);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose()
        {
            this.lifecycleHandler.Complete();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::System.Threading.Tasks.ValueTask DisposeAsync()
        {
            return this.lifecycleHandler.CompleteAsync();
        }
    }
}
