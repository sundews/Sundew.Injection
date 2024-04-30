//HintName: OverallSuccess.IConstructedChildFactory.generated.cs
#nullable enable
namespace OverallSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    public partial interface IConstructedChildFactory : global::System.IDisposable, global::System.IAsyncDisposable, global::Sundew.Injection.IGeneratedFactory
    {
        global::OverallSuccess.ChildFactory.ConstructedChild Create(global::OverallSuccess.IResolveRootFactory resolveRootFactory, global::OverallSuccess.OptionalInterface.OptionalParameters optionalParameters);

        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        global::System.Threading.Tasks.Task<global::OverallSuccess.ChildFactory.ConstructedChild> CreateAsync(global::OverallSuccess.IResolveRootFactory resolveRootFactory, global::OverallSuccess.OptionalInterface.OptionalParameters optionalParameters);

        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        global::Sundew.Injection.Constructed<global::OverallSuccess.ChildFactory.ConstructedChild> CreateUninitialized(global::OverallSuccess.IResolveRootFactory resolveRootFactory, global::OverallSuccess.OptionalInterface.OptionalParameters optionalParameters);

        void Dispose(global::OverallSuccess.ChildFactory.ConstructedChild constructedChild);

        global::System.Threading.Tasks.ValueTask DisposeAsync(global::OverallSuccess.ChildFactory.ConstructedChild constructedChild);
    }
}
