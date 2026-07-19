//HintName: DistinctSuccess.DisposableFactoriesAllowingOverrideNew.MainFactory.generated.cs
#nullable enable
namespace DistinctSuccess.DisposableFactoriesAllowingOverrideNew
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory("CreateUninitialized")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class MainFactory : global::System.IDisposable, global::System.IAsyncDisposable, global::Sundew.Injection.IGeneratedFactory
    {
        private readonly global::Sundew.Injection.ILifecycleParameters lifecycleParameters;
        private readonly global::DisposableFactoriesAllowingOverrideNew.SundewInjection.LifecycleHandler lifecycleHandler;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public MainFactory()
        {
            this.lifecycleParameters = new global::Sundew.Injection.LifecycleParameters(
                false,
                false,
                default(global::Initialization.Interfaces.IInitializationReporter),
                default(global::Disposal.Interfaces.IDisposalReporter));
            this.lifecycleHandler = new global::DisposableFactoriesAllowingOverrideNew.SundewInjection.LifecycleHandler(this.lifecycleParameters, this.lifecycleParameters);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public partial global::DistinctSuccess.DisposableFactoriesAllowingOverrideNew.Root Create()
        {
            var constructedRoot = this.CreateUninitialized();
            this.lifecycleHandler.Initialize();
            return constructedRoot.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        public async global::System.Threading.Tasks.Task<global::DistinctSuccess.DisposableFactoriesAllowingOverrideNew.Root> CreateAsync()
        {
            var constructedRoot = this.CreateUninitialized();
            await this.lifecycleHandler.InitializeAsync().ConfigureAwait(false);
            return constructedRoot.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        public global::Sundew.Injection.Constructed<global::DistinctSuccess.DisposableFactoriesAllowingOverrideNew.Root> CreateUninitialized()
        {
            var childLifecycleHandler = this.lifecycleHandler.CreateChildLifecycleHandler();
            var dependencyForRoot = this.OnCreateDependency(childLifecycleHandler);
            childLifecycleHandler.TryAdd(dependencyForRoot);
            var rootResult = new global::DistinctSuccess.DisposableFactoriesAllowingOverrideNew.Root(dependencyForRoot);
            this.lifecycleHandler.TryAdd(rootResult, childLifecycleHandler);
            return new global::Sundew.Injection.Constructed<global::DistinctSuccess.DisposableFactoriesAllowingOverrideNew.Root>(rootResult, childLifecycleHandler);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose(global::DistinctSuccess.DisposableFactoriesAllowingOverrideNew.Root root)
        {
            this.lifecycleHandler.Dispose(root);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::System.Threading.Tasks.ValueTask DisposeAsync(global::DistinctSuccess.DisposableFactoriesAllowingOverrideNew.Root root)
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

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        protected virtual global::DistinctSuccess.DisposableFactoriesAllowingOverrideNew.Dependency OnCreateDependency(global::Sundew.Injection.ILifecycleHandler lifecycleHandler)
        {
            return new global::DistinctSuccess.DisposableFactoriesAllowingOverrideNew.Dependency();
        }
    }
}
