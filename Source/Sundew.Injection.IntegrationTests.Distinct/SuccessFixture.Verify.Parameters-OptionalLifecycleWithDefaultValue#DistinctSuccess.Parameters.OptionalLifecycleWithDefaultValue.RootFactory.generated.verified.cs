//HintName: DistinctSuccess.Parameters.OptionalLifecycleWithDefaultValue.RootFactory.generated.cs
#nullable enable
namespace DistinctSuccess.Parameters.OptionalLifecycleWithDefaultValue
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory("CreateUninitialized")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class RootFactory : global::Sundew.Injection.IGeneratedFactory
    {
        private readonly global::Sundew.Injection.ILifecycleParameters lifecycleParameters;
        private readonly global::OptionalLifecycleWithDefaultValue.SundewInjection.LifecycleHandler lifecycleHandler;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public RootFactory(global::Sundew.Injection.ILifecycleParameters? lifecycleParameters = default)
        {
            this.lifecycleParameters = lifecycleParameters ?? new global::Sundew.Injection.LifecycleParameters(
                false,
                false,
                default(global::Initialization.Interfaces.IInitializationReporter),
                default(global::Disposal.Interfaces.IDisposalReporter));
            this.lifecycleHandler = new global::OptionalLifecycleWithDefaultValue.SundewInjection.LifecycleHandler(this.lifecycleParameters, this.lifecycleParameters);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public partial global::DistinctSuccess.Parameters.OptionalLifecycleWithDefaultValue.Root Create()
        {
            var constructedRoot = this.CreateUninitialized();
            this.lifecycleHandler.Initialize();
            return constructedRoot.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        public async global::System.Threading.Tasks.Task<global::DistinctSuccess.Parameters.OptionalLifecycleWithDefaultValue.Root> CreateAsync()
        {
            var constructedRoot = this.CreateUninitialized();
            await this.lifecycleHandler.InitializeAsync().ConfigureAwait(false);
            return constructedRoot.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        public global::Sundew.Injection.Constructed<global::DistinctSuccess.Parameters.OptionalLifecycleWithDefaultValue.Root> CreateUninitialized()
        {
            var childLifecycleHandler = this.lifecycleHandler.CreateChildLifecycleHandler();
            var root = new global::DistinctSuccess.Parameters.OptionalLifecycleWithDefaultValue.Root(default(global::DistinctSuccess.Parameters.OptionalLifecycleWithDefaultValue.IOption));
            childLifecycleHandler.TryAdd(root);
            var rootResult = root;
            this.lifecycleHandler.TryAdd(rootResult, childLifecycleHandler);
            return new global::Sundew.Injection.Constructed<global::DistinctSuccess.Parameters.OptionalLifecycleWithDefaultValue.Root>(rootResult, childLifecycleHandler);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        private static partial global::DistinctSuccess.Parameters.OptionalLifecycleWithDefaultValue.RootFactory Constructor(global::Sundew.Injection.ILifecycleParameters? lifecycleParameters)
        {
            return new global::DistinctSuccess.Parameters.OptionalLifecycleWithDefaultValue.RootFactory(lifecycleParameters);
        }
    }
}
