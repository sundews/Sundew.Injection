﻿//HintName: AllFeaturesSuccess.ConstructedChildFactory.generated.cs
#nullable enable
namespace AllFeaturesSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class ConstructedChildFactory : global::AllFeaturesSuccess.IConstructedChildFactory
    {
        private readonly global::Sundew.Injection.ILifecycleParameters lifecycleParameters;
        private readonly global::Sundew.Injection.LifecycleHandler lifecycleHandler;
        private readonly global::AllFeaturesSuccessDependency.DependencyFactory dependencyFactory;
        private readonly global::AllFeaturesSuccessDependency.ManualDependencyFactory manualDependencyFactory;

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
            this.dependencyFactory = new global::AllFeaturesSuccessDependency.DependencyFactory();
            this.lifecycleHandler.TryAdd(this.dependencyFactory);
            this.manualDependencyFactory = new global::AllFeaturesSuccessDependency.ManualDependencyFactory();
        }

        public global::AllFeaturesSuccess.ChildFactory.ConstructedChild Create(global::AllFeaturesSuccess.OptionalInterface.OptionalParameters optionalParameters)
        {
            var constructedConstructedChild = this.CreateUninitialized(optionalParameters);
            this.lifecycleHandler.Initialize();
            return constructedConstructedChild.Object;
        }

        public async global::System.Threading.Tasks.Task<global::AllFeaturesSuccess.ChildFactory.ConstructedChild> CreateAsync(global::AllFeaturesSuccess.OptionalInterface.OptionalParameters optionalParameters)
        {
            var constructedConstructedChild = this.CreateUninitialized(optionalParameters);
            await this.lifecycleHandler.InitializeAsync().ConfigureAwait(false);
            return constructedConstructedChild.Object;
        }

        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.CreateMethod]
        public global::Sundew.Injection.Constructed<global::AllFeaturesSuccess.ChildFactory.ConstructedChild> CreateUninitialized(global::AllFeaturesSuccess.OptionalInterface.OptionalParameters optionalParameters)
        {
            var childLifecycleHandler = this.lifecycleHandler.CreateChildLifecycleHandler();
            var newInstanceAndDisposableForConstructedChild = optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? new global::AllFeaturesSuccess.NewInstance.NewInstanceAndDisposable(default(global::AllFeaturesSuccess.OptionalInterface.IOmittedOptional));
            childLifecycleHandler.TryAdd(newInstanceAndDisposableForConstructedChild);
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

        public void Dispose(global::AllFeaturesSuccess.ChildFactory.ConstructedChild constructedChild)
        {
            this.lifecycleHandler.Dispose(constructedChild);
        }

        public global::System.Threading.Tasks.ValueTask DisposeAsync(global::AllFeaturesSuccess.ChildFactory.ConstructedChild constructedChild)
        {
            return this.lifecycleHandler.DisposeAsync(constructedChild);
        }

        public void Dispose()
        {
            this.lifecycleHandler.Dispose();
        }

        public global::System.Threading.Tasks.ValueTask DisposeAsync()
        {
            return this.lifecycleHandler.DisposeAsync();
        }
    }
}