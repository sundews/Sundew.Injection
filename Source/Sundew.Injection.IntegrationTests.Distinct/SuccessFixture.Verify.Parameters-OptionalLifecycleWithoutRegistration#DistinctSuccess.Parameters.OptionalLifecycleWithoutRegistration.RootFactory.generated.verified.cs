//HintName: DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration.RootFactory.generated.cs
#nullable enable
namespace DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory("CreateUninitialized")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class RootFactory : global::System.IDisposable, global::System.IAsyncDisposable, global::Sundew.Injection.IGeneratedFactory
    {
        private readonly global::Sundew.Injection.ILifecycleParameters lifecycleParameters;
        private readonly global::OptionalLifecycleWithoutRegistration.SundewInjection.LifecycleHandler lifecycleHandler;
        private readonly global::DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration.IRequired1 required1;
        private readonly global::DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration.IRequired2 required2;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public RootFactory(global::DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration.IRequired1 required1, global::DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration.IRequired2 required2, global::Sundew.Injection.ILifecycleParameters? lifecycleParameters = default)
        {
            this.lifecycleParameters = lifecycleParameters ?? new global::Sundew.Injection.LifecycleParameters(
                false,
                false,
                default(global::Initialization.Interfaces.IInitializationReporter),
                default(global::Disposal.Interfaces.IDisposalReporter));
            this.lifecycleHandler = new global::OptionalLifecycleWithoutRegistration.SundewInjection.LifecycleHandler(this.lifecycleParameters, this.lifecycleParameters);
            this.required1 = required1;
            this.required2 = required2;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public partial global::DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration.Root Create()
        {
            var constructedRoot = this.CreateUninitialized();
            this.lifecycleHandler.Initialize();
            return constructedRoot.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        public async global::System.Threading.Tasks.Task<global::DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration.Root> CreateAsync()
        {
            var constructedRoot = this.CreateUninitialized();
            await this.lifecycleHandler.InitializeAsync().ConfigureAwait(false);
            return constructedRoot.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        public global::Sundew.Injection.Constructed<global::DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration.Root> CreateUninitialized()
        {
            var childLifecycleHandler = this.lifecycleHandler.CreateChildLifecycleHandler();
            var root = new global::DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration.Root(this.required1, this.required2);
            childLifecycleHandler.TryAdd(root);
            var rootResult = root;
            this.lifecycleHandler.TryAdd(rootResult, childLifecycleHandler);
            return new global::Sundew.Injection.Constructed<global::DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration.Root>(rootResult, childLifecycleHandler);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose(global::DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration.Root root)
        {
            this.lifecycleHandler.Dispose(root);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::System.Threading.Tasks.ValueTask DisposeAsync(global::DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration.Root root)
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
        private static partial global::DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration.RootFactory Constructor(global::DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration.IRequired1 required1, global::DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration.IRequired2 required2, global::Sundew.Injection.ILifecycleParameters? lifecycleParameters)
        {
            return new global::DistinctSuccess.Parameters.OptionalLifecycleWithoutRegistration.RootFactory(required1, required2, lifecycleParameters);
        }
    }
}
