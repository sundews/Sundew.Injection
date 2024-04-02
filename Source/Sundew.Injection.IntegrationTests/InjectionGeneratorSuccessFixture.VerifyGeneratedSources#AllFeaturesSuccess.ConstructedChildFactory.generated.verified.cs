//HintName: AllFeaturesSuccess.ConstructedChildFactory.generated.cs
#nullable enable
namespace AllFeaturesSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class ConstructedChildFactory : global::AllFeaturesSuccess.IConstructedChildFactory
    {
        private readonly global::Sundew.Injection.ILifecycleParameters lifecycleParameters;
        private readonly global::Sundew.Injection.LifecycleHandler lifecycleHandler;
        private readonly global::AllFeaturesSuccessDependency.DependencyFactory dependencyFactory;
        private readonly global::AllFeaturesSuccessDependency.ManualDependencyFactory manualDependencyFactory;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public ConstructedChildFactory(global::Sundew.Injection.ILifecycleParameters? lifecycleParameters = null)
        {
            this.lifecycleParameters = lifecycleParameters ?? new global::Sundew.Injection.LifecycleParameters(
                false,
                false,
                default(global::Initialization.Interfaces.IInitializationReporter),
                default(global::Disposal.Interfaces.IDisposalReporter));
            this.lifecycleHandler = new global::Sundew.Injection.LifecycleHandler(this.lifecycleParameters, this.lifecycleParameters);
            this.dependencyFactory = new global::AllFeaturesSuccessDependency.DependencyFactory();
            this.lifecycleHandler.TryAdd(this.dependencyFactory);
            this.manualDependencyFactory = new global::AllFeaturesSuccessDependency.ManualDependencyFactory();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::AllFeaturesSuccess.ChildFactory.ConstructedChild Create(global::AllFeaturesSuccess.IResolveRootFactory resolveRootFactory, global::AllFeaturesSuccess.OptionalInterface.OptionalParameters optionalParameters)
        {
            var constructedConstructedChild = this.CreateUninitialized(resolveRootFactory, optionalParameters);
            this.lifecycleHandler.Initialize();
            return constructedConstructedChild.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.IndirectCreateMethodAttribute]
        public async global::System.Threading.Tasks.Task<global::AllFeaturesSuccess.ChildFactory.ConstructedChild> CreateAsync(global::AllFeaturesSuccess.IResolveRootFactory resolveRootFactory, global::AllFeaturesSuccess.OptionalInterface.OptionalParameters optionalParameters)
        {
            var constructedConstructedChild = this.CreateUninitialized(resolveRootFactory, optionalParameters);
            await this.lifecycleHandler.InitializeAsync().ConfigureAwait(false);
            return constructedConstructedChild.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        [global::Sundew.Injection.IndirectCreateMethodAttribute]
        public global::Sundew.Injection.Constructed<global::AllFeaturesSuccess.ChildFactory.ConstructedChild> CreateUninitialized(global::AllFeaturesSuccess.IResolveRootFactory resolveRootFactory, global::AllFeaturesSuccess.OptionalInterface.OptionalParameters optionalParameters)
        {
            var childLifecycleHandler = this.lifecycleHandler.CreateChildLifecycleHandler();
            var newInstanceAndDisposableForConstructedChild = optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? childLifecycleHandler.TryAdd(new global::AllFeaturesSuccess.NewInstance.NewInstanceAndDisposable(resolveRootFactory, default(global::AllFeaturesSuccess.OptionalInterface.IOmittedOptional)));
            var constructedDependencyForConstructedChild = this.dependencyFactory.CreateUninitialized();
            childLifecycleHandler.TryAdd(constructedDependencyForConstructedChild);
            var dependencyForConstructedChild = constructedDependencyForConstructedChild.Object;
            childLifecycleHandler.TryAdd(dependencyForConstructedChild);
            var manualDependencyForConstructedChild = this.manualDependencyFactory.Create();
            childLifecycleHandler.TryAdd(manualDependencyForConstructedChild);
            var constructedChild = new global::AllFeaturesSuccess.ChildFactory.ConstructedChild(newInstanceAndDisposableForConstructedChild, dependencyForConstructedChild, manualDependencyForConstructedChild);
            childLifecycleHandler.TryAdd(constructedChild);
            var constructedChildResult = constructedChild;
            this.lifecycleHandler.TryAdd(constructedChildResult, childLifecycleHandler);
            return new global::Sundew.Injection.Constructed<global::AllFeaturesSuccess.ChildFactory.ConstructedChild>(constructedChildResult, childLifecycleHandler);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose(global::AllFeaturesSuccess.ChildFactory.ConstructedChild constructedChild)
        {
            this.lifecycleHandler.Dispose(constructedChild);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::System.Threading.Tasks.ValueTask DisposeAsync(global::AllFeaturesSuccess.ChildFactory.ConstructedChild constructedChild)
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
