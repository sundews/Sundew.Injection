//HintName: Success.IConstructedChildFactory.generated.cs
#nullable enable
namespace Success
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    public partial interface IConstructedChildFactory : global::System.IDisposable, global::System.IAsyncDisposable, global::Sundew.Injection.IGeneratedFactory
    {
        global::Success.ChildFactory.ConstructedChild Create(global::Success.IResolveRootFactory resolveRootFactory, global::Success.OptionalInterface.OptionalParameters optionalParameters);

        [global::Sundew.Injection.IndirectCreateMethodAttribute]
        global::System.Threading.Tasks.Task<global::Success.ChildFactory.ConstructedChild> CreateAsync(global::Success.IResolveRootFactory resolveRootFactory, global::Success.OptionalInterface.OptionalParameters optionalParameters);

        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        [global::Sundew.Injection.IndirectCreateMethodAttribute]
        global::Sundew.Injection.Constructed<global::Success.ChildFactory.ConstructedChild> CreateUninitialized(global::Success.IResolveRootFactory resolveRootFactory, global::Success.OptionalInterface.OptionalParameters optionalParameters);

        void Dispose(global::Success.ChildFactory.ConstructedChild constructedChild);

        global::System.Threading.Tasks.ValueTask DisposeAsync(global::Success.ChildFactory.ConstructedChild constructedChild);
    }
}
