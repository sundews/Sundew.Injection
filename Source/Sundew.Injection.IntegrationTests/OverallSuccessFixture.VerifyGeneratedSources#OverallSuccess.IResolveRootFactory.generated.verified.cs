//HintName: OverallSuccess.IResolveRootFactory.generated.cs
#nullable enable
namespace OverallSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    public partial interface IResolveRootFactory : global::System.IDisposable, global::System.IAsyncDisposable, global::Sundew.Injection.IGeneratedFactory
    {
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        global::OverallSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory InterfaceSingleInstancePerFactory { get; }

        global::OverallSuccess.IResolveRoot CreateResolveRoot(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::OverallSuccess.RequiredInterface.IRequiredService> requiredService,
            global::OverallSuccess.RequiredInterface.RequiredParameter requiredParameter,
            global::OverallSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::OverallSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null);

        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        global::System.Threading.Tasks.Task<global::OverallSuccess.IResolveRoot> CreateResolveRootAsync(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::OverallSuccess.RequiredInterface.IRequiredService> requiredService,
            global::OverallSuccess.RequiredInterface.RequiredParameter requiredParameter,
            global::OverallSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::OverallSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null);

        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        global::Sundew.Injection.Constructed<global::OverallSuccess.IResolveRoot> CreateResolveRootUninitialized(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::OverallSuccess.RequiredInterface.IRequiredService> requiredService,
            global::OverallSuccess.RequiredInterface.RequiredParameter requiredParameter,
            global::OverallSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::OverallSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null);

        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        global::OverallSuccess.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverC();

        void Dispose(global::OverallSuccess.IResolveRoot resolveRoot);

        global::System.Threading.Tasks.ValueTask DisposeAsync(global::OverallSuccess.IResolveRoot resolveRoot);
    }
}
