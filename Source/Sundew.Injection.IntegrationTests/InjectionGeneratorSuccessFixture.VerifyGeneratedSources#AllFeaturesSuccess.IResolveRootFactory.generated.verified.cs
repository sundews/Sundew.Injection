//HintName: AllFeaturesSuccess.IResolveRootFactory.generated.cs
#nullable enable
namespace AllFeaturesSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    public partial interface IResolveRootFactory : global::System.IDisposable, global::System.IAsyncDisposable, global::Sundew.Injection.IGeneratedFactory
    {
        global::AllFeaturesSuccess.IResolveRoot CreateResolveRoot(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::AllFeaturesSuccess.RequiredInterface.IRequiredService> requiredService,
            global::AllFeaturesSuccess.RequiredInterface.RequiredParameter requiredParameter,
            global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null);

        [global::Sundew.Injection.IndirectCreateMethodAttribute]
        global::System.Threading.Tasks.Task<global::AllFeaturesSuccess.IResolveRoot> CreateResolveRootAsync(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::AllFeaturesSuccess.RequiredInterface.IRequiredService> requiredService,
            global::AllFeaturesSuccess.RequiredInterface.RequiredParameter requiredParameter,
            global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null);

        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        [global::Sundew.Injection.IndirectCreateMethodAttribute]
        global::Sundew.Injection.Constructed<global::AllFeaturesSuccess.IResolveRoot> CreateResolveRootUninitialized(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::AllFeaturesSuccess.RequiredInterface.IRequiredService> requiredService,
            global::AllFeaturesSuccess.RequiredInterface.RequiredParameter requiredParameter,
            global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null);

        [global::Sundew.Injection.BindableCreateMethodAttribute]
        global::AllFeaturesSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory CreateInterfaceSingleInstancePerFactory();

        [global::Sundew.Injection.BindableCreateMethodAttribute]
        global::AllFeaturesSuccess.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverC();

        void Dispose(global::AllFeaturesSuccess.IResolveRoot resolveRoot);

        global::System.Threading.Tasks.ValueTask DisposeAsync(global::AllFeaturesSuccess.IResolveRoot resolveRoot);
    }
}
