//HintName: ConstructedChildFactory.cs
namespace AllFeaturesSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.0.0.0")]
    [global::Sundew.Injection.Factory]
    public sealed class ConstructedChildFactory : global::AllFeaturesSuccess.IConstructedChildFactory
    {
        private readonly global::Sundew.Injection.ILifecycleParameters lifecycleParameters;
        private readonly global::Sundew.Injection.LifecycleHandler lifecycleHandler;

        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public ConstructedChildFactory(global::Sundew.Injection.ILifecycleParameters? lifecycleParameters = null)
        {
            if (lifecycleParameters == null)
            {
                var ownedLifecycleParameters = new global::Sundew.Injection.LifecycleParameters(
                    false,
                    false,
                    default(global::Initialization.Interfaces.IInitializationReporter),
                    default(global::Disposal.Interfaces.IDisposalReporter));
                this.lifecycleParameters = ownedLifecycleParameters;
            }
            else
            {
                this.lifecycleParameters = lifecycleParameters;
            }

            this.lifecycleHandler = new global::Sundew.Injection.LifecycleHandler(this.lifecycleParameters, this.lifecycleParameters);
        }

        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public global::AllFeaturesSuccess.ChildFactory.ConstructedChild Create(global::AllFeaturesSuccess.OptionalInterface.OptionalParameters optionalParameters)
        {
            var constructedConstructedChild = this.CreateUninitialized(optionalParameters);
            this.lifecycleHandler.Initialize();
            return constructedConstructedChild.Object;
        }

        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public async global::System.Threading.Tasks.Task<global::AllFeaturesSuccess.ChildFactory.ConstructedChild> CreateAsync(global::AllFeaturesSuccess.OptionalInterface.OptionalParameters optionalParameters)
        {
            var constructedConstructedChild = this.CreateUninitialized(optionalParameters);
            await this.lifecycleHandler.InitializeAsync().ConfigureAwait(false);
            return constructedConstructedChild.Object;
        }

        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.CreateMethod]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public global::Sundew.Injection.Constructed<global::AllFeaturesSuccess.ChildFactory.ConstructedChild> CreateUninitialized(global::AllFeaturesSuccess.OptionalInterface.OptionalParameters optionalParameters)
        {
            var childLifecycleHandler = this.lifecycleHandler.CreateChildLifecycleHandler();
            var newInstanceAndDisposableForConstructedChild = optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? new global::AllFeaturesSuccess.NewInstance.NewInstanceAndDisposable(default(global::AllFeaturesSuccess.OptionalInterface.IOmittedOptional));
            childLifecycleHandler.TryAdd(newInstanceAndDisposableForConstructedChild);
            var dependencyFactory = new DependencyFactory();
            var dependencyConstructed = dependencyFactory.Create();
            var dependencyForConstructedChild = dependencyConstructed.Object;
            childLifecycleHandler.TryAdd(dependencyForConstructedChild);
            var constructedChild = new global::AllFeaturesSuccess.ChildFactory.ConstructedChild(newInstanceAndDisposableForConstructedChild, dependencyForConstructedChild);
            childLifecycleHandler.TryAdd(constructedChild);
            var constructedChildResult = constructedChild;
            this.lifecycleHandler.TryAdd(constructedChildResult, childLifecycleHandler);
            return new global::Sundew.Injection.Constructed<global::AllFeaturesSuccess.ChildFactory.ConstructedChild>(constructedChildResult, childLifecycleHandler);
        }

        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void Dispose(global::AllFeaturesSuccess.ChildFactory.ConstructedChild constructedChild)
        {
            this.lifecycleHandler.Dispose(constructedChild);
        }

        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public global::System.Threading.Tasks.ValueTask DisposeAsync(global::AllFeaturesSuccess.ChildFactory.ConstructedChild constructedChild)
        {
            return this.lifecycleHandler.DisposeAsync(constructedChild);
        }

        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void Dispose()
        {
            this.lifecycleHandler.Dispose();
        }

        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public global::System.Threading.Tasks.ValueTask DisposeAsync()
        {
            return this.lifecycleHandler.DisposeAsync();
        }
    }
}
