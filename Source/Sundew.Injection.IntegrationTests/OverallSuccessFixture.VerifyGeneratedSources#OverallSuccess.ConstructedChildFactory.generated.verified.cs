//HintName: OverallSuccess.ConstructedChildFactory.generated.cs
#nullable enable
namespace OverallSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class ConstructedChildFactory : global::OverallSuccess.IConstructedChildFactory
    {
        private readonly global::Sundew.Injection.ILifecycleParameters lifecycleParameters;
        private readonly global::Sundew.Injection.LifecycleHandler lifecycleHandler;
        private readonly global::OverallSuccessDependency.DependencyFactory dependencyFactory;
        private readonly global::OverallSuccessDependency.ManualMultipleDependencyFactory manualMultipleDependencyFactory;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public ConstructedChildFactory(global::Sundew.Injection.ILifecycleParameters? lifecycleParameters = null)
        {
            this.lifecycleParameters = lifecycleParameters ?? new global::Sundew.Injection.LifecycleParameters(
                false,
                false,
                default(global::Initialization.Interfaces.IInitializationReporter),
                default(global::Disposal.Interfaces.IDisposalReporter));
            this.lifecycleHandler = new global::Sundew.Injection.LifecycleHandler(this.lifecycleParameters, this.lifecycleParameters);
            this.dependencyFactory = new global::OverallSuccessDependency.DependencyFactory();
            this.lifecycleHandler.TryAdd(this.dependencyFactory);
            this.manualMultipleDependencyFactory = new global::OverallSuccessDependency.ManualMultipleDependencyFactory();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::OverallSuccess.ChildFactory.ConstructedChild Create(global::OverallSuccess.IResolveRootFactory resolveRootFactory, global::OverallSuccess.OptionalInterface.OptionalParameters optionalParameters)
        {
            var constructedConstructedChild = this.CreateUninitialized(resolveRootFactory, optionalParameters);
            this.lifecycleHandler.Initialize();
            return constructedConstructedChild.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        public async global::System.Threading.Tasks.Task<global::OverallSuccess.ChildFactory.ConstructedChild> CreateAsync(global::OverallSuccess.IResolveRootFactory resolveRootFactory, global::OverallSuccess.OptionalInterface.OptionalParameters optionalParameters)
        {
            var constructedConstructedChild = this.CreateUninitialized(resolveRootFactory, optionalParameters);
            await this.lifecycleHandler.InitializeAsync().ConfigureAwait(false);
            return constructedConstructedChild.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        public global::Sundew.Injection.Constructed<global::OverallSuccess.ChildFactory.ConstructedChild> CreateUninitialized(global::OverallSuccess.IResolveRootFactory resolveRootFactory, global::OverallSuccess.OptionalInterface.OptionalParameters optionalParameters)
        {
            var childLifecycleHandler = this.lifecycleHandler.CreateChildLifecycleHandler();
            var newInstanceAndDisposableForConstructedChild = optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? childLifecycleHandler.TryAdd(new global::OverallSuccess.NewInstance.NewInstanceAndDisposable(resolveRootFactory, default(global::OverallSuccess.OptionalInterface.IOmittedOptional)));
            var constructedChild = new global::OverallSuccess.ChildFactory.ConstructedChild(newInstanceAndDisposableForConstructedChild, this.dependencyFactory.CreateUninitialized().Object, this.manualMultipleDependencyFactory.CreateNewInstance());
            childLifecycleHandler.TryAdd(constructedChild);
            var constructedChildResult = constructedChild;
            this.lifecycleHandler.TryAdd(constructedChildResult, childLifecycleHandler);
            return new global::Sundew.Injection.Constructed<global::OverallSuccess.ChildFactory.ConstructedChild>(constructedChildResult, childLifecycleHandler);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose(global::OverallSuccess.ChildFactory.ConstructedChild constructedChild)
        {
            this.lifecycleHandler.Dispose(constructedChild);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::System.Threading.Tasks.ValueTask DisposeAsync(global::OverallSuccess.ChildFactory.ConstructedChild constructedChild)
        {
            return this.lifecycleHandler.DisposeAsync(constructedChild);
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
