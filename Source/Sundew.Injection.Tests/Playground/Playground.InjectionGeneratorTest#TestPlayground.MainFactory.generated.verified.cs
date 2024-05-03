//HintName: TestPlayground.MainFactory.generated.cs
#nullable enable
namespace TestPlayground
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class MainFactory : global::System.IDisposable, global::System.IAsyncDisposable, global::Sundew.Injection.IGeneratedFactory
    {
        private readonly global::TestPlayground.SundewInjection.LifecycleHandler lifecycleHandler;
        private readonly global::TestPlayground.DependencyFactory dependencyFactory;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public MainFactory()
        {
            this.lifecycleHandler = new global::TestPlayground.SundewInjection.LifecycleHandler(default(global::Sundew.Injection.IInitializationParameters), default(global::Sundew.Injection.IDisposalParameters));
            this.dependencyFactory = new global::TestPlayground.DependencyFactory();
            this.lifecycleHandler.TryAdd(this.dependencyFactory);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::TestPlayground.Root Create()
        {
            var constructedRoot = this.CreateUninitialized();
            this.lifecycleHandler.Initialize();
            return constructedRoot.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        public async global::System.Threading.Tasks.Task<global::TestPlayground.Root> CreateAsync()
        {
            var constructedRoot = this.CreateUninitialized();
            await this.lifecycleHandler.InitializeAsync().ConfigureAwait(false);
            return constructedRoot.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        public global::Sundew.Injection.Constructed<global::TestPlayground.Root> CreateUninitialized()
        {
            var childLifecycleHandler = this.lifecycleHandler.CreateChildLifecycleHandler();
            var intermediateForRoot = new global::TestPlayground.Intermediate(this.dependencyFactory.Dependency);
            childLifecycleHandler.TryAdd(intermediateForRoot);
            var rootResult = new global::TestPlayground.Root(intermediateForRoot);
            this.lifecycleHandler.TryAdd(rootResult, childLifecycleHandler);
            return new global::Sundew.Injection.Constructed<global::TestPlayground.Root>(rootResult, childLifecycleHandler);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose(global::TestPlayground.Root root)
        {
            this.lifecycleHandler.Dispose(root);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::System.Threading.Tasks.ValueTask DisposeAsync(global::TestPlayground.Root root)
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
