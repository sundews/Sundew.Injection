//HintName: DistinctSuccess.IntermediateDependencyFromBoundFactoryWithInitializable.MainFactory.generated.cs
#nullable enable
namespace DistinctSuccess.IntermediateDependencyFromBoundFactoryWithInitializable
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class MainFactory : global::System.IDisposable, global::System.IAsyncDisposable, global::Sundew.Injection.IGeneratedFactory
    {
        private readonly global::DistinctSuccess.SundewInjection.LifecycleHandler lifecycleHandler;
        private readonly global::DistinctSuccess.IntermediateDependencyFromBoundFactoryWithInitializable.DependencyFactory dependencyFactory;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public MainFactory()
        {
            this.lifecycleHandler = new global::DistinctSuccess.SundewInjection.LifecycleHandler(default(global::Sundew.Injection.IInitializationParameters), default(global::Sundew.Injection.IDisposalParameters));
            this.dependencyFactory = new global::DistinctSuccess.IntermediateDependencyFromBoundFactoryWithInitializable.DependencyFactory();
            this.lifecycleHandler.TryAdd(this.dependencyFactory);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::DistinctSuccess.IntermediateDependencyFromBoundFactoryWithInitializable.Root Create()
        {
            var constructedRoot = this.CreateUninitialized();
            this.lifecycleHandler.Initialize();
            return constructedRoot.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        public async global::System.Threading.Tasks.Task<global::DistinctSuccess.IntermediateDependencyFromBoundFactoryWithInitializable.Root> CreateAsync()
        {
            var constructedRoot = this.CreateUninitialized();
            await this.lifecycleHandler.InitializeAsync().ConfigureAwait(false);
            return constructedRoot.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        public global::Sundew.Injection.Constructed<global::DistinctSuccess.IntermediateDependencyFromBoundFactoryWithInitializable.Root> CreateUninitialized()
        {
            var childLifecycleHandler = this.lifecycleHandler.CreateChildLifecycleHandler();
            var intermediateForRoot = new global::DistinctSuccess.IntermediateDependencyFromBoundFactoryWithInitializable.Intermediate(this.dependencyFactory.Dependency);
            childLifecycleHandler.TryAdd(intermediateForRoot);
            var rootResult = new global::DistinctSuccess.IntermediateDependencyFromBoundFactoryWithInitializable.Root(intermediateForRoot);
            this.lifecycleHandler.TryAdd(rootResult, childLifecycleHandler);
            return new global::Sundew.Injection.Constructed<global::DistinctSuccess.IntermediateDependencyFromBoundFactoryWithInitializable.Root>(rootResult, childLifecycleHandler);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose(global::DistinctSuccess.IntermediateDependencyFromBoundFactoryWithInitializable.Root root)
        {
            this.lifecycleHandler.Dispose(root);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::System.Threading.Tasks.ValueTask DisposeAsync(global::DistinctSuccess.IntermediateDependencyFromBoundFactoryWithInitializable.Root root)
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
