//HintName: AllFeaturesSuccess.IResolveRootFactory.generated.cs
#nullable enable
namespace AllFeaturesSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    public interface IResolveRootFactory : global::System.IDisposable, global::System.IAsyncDisposable, global::Sundew.Injection.IGeneratedFactory
    {
        global::AllFeaturesSuccess.IResolveRoot CreateResolveRoot(
            int[] defaultItems,
            global::System.Func<global::AllFeaturesSuccess.RequiredInterface.IRequiredService> requiredService,
            global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null);

        global::System.Threading.Tasks.Task<global::AllFeaturesSuccess.IResolveRoot> CreateResolveRootAsync(
            int[] defaultItems,
            global::System.Func<global::AllFeaturesSuccess.RequiredInterface.IRequiredService> requiredService,
            global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null);

        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.CreateMethod]
        global::Sundew.Injection.Constructed<global::AllFeaturesSuccess.IResolveRoot> CreateResolveRootUninitialized(
            int[] defaultItems,
            global::System.Func<global::AllFeaturesSuccess.RequiredInterface.IRequiredService> requiredService,
            global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null);

        [global::Sundew.Injection.CreateMethod]
        global::AllFeaturesSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory CreateInterfaceSingleInstancePerFactory();

        void Dispose(global::AllFeaturesSuccess.IResolveRoot resolveRoot);

        global::System.Threading.Tasks.ValueTask DisposeAsync(global::AllFeaturesSuccess.IResolveRoot resolveRoot);
    }
}
