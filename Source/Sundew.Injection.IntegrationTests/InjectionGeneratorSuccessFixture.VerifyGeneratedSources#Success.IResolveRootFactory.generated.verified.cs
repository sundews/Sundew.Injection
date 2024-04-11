//HintName: Success.IResolveRootFactory.generated.cs
#nullable enable
namespace Success
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    public partial interface IResolveRootFactory : global::System.IDisposable, global::System.IAsyncDisposable, global::Sundew.Injection.IGeneratedFactory
    {
        global::Success.IResolveRoot CreateResolveRoot(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::Success.RequiredInterface.IRequiredService> requiredService,
            global::Success.RequiredInterface.RequiredParameter requiredParameter,
            global::Success.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::Success.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null);

        [global::Sundew.Injection.IndirectCreateMethodAttribute]
        global::System.Threading.Tasks.Task<global::Success.IResolveRoot> CreateResolveRootAsync(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::Success.RequiredInterface.IRequiredService> requiredService,
            global::Success.RequiredInterface.RequiredParameter requiredParameter,
            global::Success.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::Success.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null);

        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        [global::Sundew.Injection.IndirectCreateMethodAttribute]
        global::Sundew.Injection.Constructed<global::Success.IResolveRoot> CreateResolveRootUninitialized(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::Success.RequiredInterface.IRequiredService> requiredService,
            global::Success.RequiredInterface.RequiredParameter requiredParameter,
            global::Success.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::Success.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null);

        [global::Sundew.Injection.BindableCreateMethodAttribute]
        global::Success.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory CreateInterfaceSingleInstancePerFactory();

        [global::Sundew.Injection.BindableCreateMethodAttribute]
        global::Success.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverC();

        void Dispose(global::Success.IResolveRoot resolveRoot);

        global::System.Threading.Tasks.ValueTask DisposeAsync(global::Success.IResolveRoot resolveRoot);
    }
}
