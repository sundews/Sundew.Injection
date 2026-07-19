//HintName: DistinctSuccess.DisposableDependency.MainFactory.generated.cs
#nullable enable
namespace DistinctSuccess.DisposableDependency
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory("Create")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class MainFactory : global::System.IDisposable, global::System.IAsyncDisposable, global::Sundew.Injection.IGeneratedFactory
    {
        private readonly global::Sundew.Injection.ILifecycleParameters lifecycleParameters;
        private readonly global::DisposableDependency.SundewInjection.LifecycleHandler lifecycleHandler;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public MainFactory()
        {
            this.lifecycleParameters = new global::Sundew.Injection.LifecycleParameters(
                false,
                false,
                default(global::Initialization.Interfaces.IInitializationReporter),
                default(global::Disposal.Interfaces.IDisposalReporter));
            this.lifecycleHandler = new global::DisposableDependency.SundewInjection.LifecycleHandler(this.lifecycleParameters, this.lifecycleParameters);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public partial global::DistinctSuccess.DisposableDependency.Root Create()
        {
            var childLifecycleHandler = this.lifecycleHandler.CreateChildLifecycleHandler();
            var dependencyForRoot = new global::DistinctSuccess.DisposableDependency.Dependency();
            childLifecycleHandler.TryAdd(dependencyForRoot);
            var rootResult = new global::DistinctSuccess.DisposableDependency.Root(dependencyForRoot);
            this.lifecycleHandler.TryAdd(rootResult, childLifecycleHandler);
            return rootResult;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose(global::DistinctSuccess.DisposableDependency.Root root)
        {
            this.lifecycleHandler.Dispose(root);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::System.Threading.Tasks.ValueTask DisposeAsync(global::DistinctSuccess.DisposableDependency.Root root)
        {
            return this.lifecycleHandler.DisposeAsync(root);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose()
        {
            this.lifecycleHandler.Dispose();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::System.Threading.Tasks.ValueTask DisposeAsync()
        {
            return this.lifecycleHandler.DisposeAsync();
        }
    }
}
