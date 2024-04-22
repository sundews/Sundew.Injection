//HintName: Success.ConstructedChildFactory.generated.cs
#nullable enable
namespace Success
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class ConstructedChildFactory : global::Success.IConstructedChildFactory
    {
        private readonly global::Sundew.Injection.ILifecycleParameters lifecycleParameters;
        private readonly global::Sundew.Injection.LifecycleHandler lifecycleHandler;
        private readonly global::SuccessDependency.DependencyFactory dependencyFactory;
        private readonly global::SuccessDependency.ManualDependencyFactory manualDependencyFactory;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public ConstructedChildFactory(global::Sundew.Injection.ILifecycleParameters? lifecycleParameters = null)
        {
            this.lifecycleParameters = lifecycleParameters ?? new global::Sundew.Injection.LifecycleParameters(
                false,
                false,
                default(global::Initialization.Interfaces.IInitializationReporter),
                default(global::Disposal.Interfaces.IDisposalReporter));
            this.lifecycleHandler = new global::Sundew.Injection.LifecycleHandler(this.lifecycleParameters, this.lifecycleParameters);
            this.dependencyFactory = new global::SuccessDependency.DependencyFactory();
            this.lifecycleHandler.TryAdd(this.dependencyFactory);
            this.manualDependencyFactory = new global::SuccessDependency.ManualDependencyFactory();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::Success.ChildFactory.ConstructedChild Create(global::Success.IResolveRootFactory resolveRootFactory, global::Success.OptionalInterface.OptionalParameters optionalParameters)
        {
            var constructedConstructedChild = this.CreateUninitialized(resolveRootFactory, optionalParameters);
            this.lifecycleHandler.Initialize();
            return constructedConstructedChild.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        public async global::System.Threading.Tasks.Task<global::Success.ChildFactory.ConstructedChild> CreateAsync(global::Success.IResolveRootFactory resolveRootFactory, global::Success.OptionalInterface.OptionalParameters optionalParameters)
        {
            var constructedConstructedChild = this.CreateUninitialized(resolveRootFactory, optionalParameters);
            await this.lifecycleHandler.InitializeAsync().ConfigureAwait(false);
            return constructedConstructedChild.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        public global::Sundew.Injection.Constructed<global::Success.ChildFactory.ConstructedChild> CreateUninitialized(global::Success.IResolveRootFactory resolveRootFactory, global::Success.OptionalInterface.OptionalParameters optionalParameters)
        {
            var childLifecycleHandler = this.lifecycleHandler.CreateChildLifecycleHandler();
            var newInstanceAndDisposableForConstructedChild = optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? childLifecycleHandler.TryAdd(new global::Success.NewInstance.NewInstanceAndDisposable(resolveRootFactory, default(global::Success.OptionalInterface.IOmittedOptional)));
            var constructedDependencyForConstructedChild = this.dependencyFactory.CreateUninitialized();
            childLifecycleHandler.TryAdd(constructedDependencyForConstructedChild);
            var dependencyForConstructedChild = constructedDependencyForConstructedChild.Object;
            childLifecycleHandler.TryAdd(dependencyForConstructedChild);
            var manualDependencyForConstructedChild = this.manualDependencyFactory.Create();
            childLifecycleHandler.TryAdd(manualDependencyForConstructedChild);
            var constructedChild = new global::Success.ChildFactory.ConstructedChild(newInstanceAndDisposableForConstructedChild, dependencyForConstructedChild, manualDependencyForConstructedChild);
            childLifecycleHandler.TryAdd(constructedChild);
            var constructedChildResult = constructedChild;
            this.lifecycleHandler.TryAdd(constructedChildResult, childLifecycleHandler);
            return new global::Sundew.Injection.Constructed<global::Success.ChildFactory.ConstructedChild>(constructedChildResult, childLifecycleHandler);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose(global::Success.ChildFactory.ConstructedChild constructedChild)
        {
            this.lifecycleHandler.Dispose(constructedChild);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::System.Threading.Tasks.ValueTask DisposeAsync(global::Success.ChildFactory.ConstructedChild constructedChild)
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
