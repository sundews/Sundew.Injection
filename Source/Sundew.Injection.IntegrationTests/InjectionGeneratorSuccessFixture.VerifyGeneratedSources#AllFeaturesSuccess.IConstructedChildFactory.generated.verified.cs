//HintName: AllFeaturesSuccess.IConstructedChildFactory.generated.cs
#nullable enable
namespace AllFeaturesSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    public partial interface IConstructedChildFactory : global::System.IDisposable, global::System.IAsyncDisposable, global::Sundew.Injection.IGeneratedFactory
    {
        global::AllFeaturesSuccess.ChildFactory.ConstructedChild Create(global::AllFeaturesSuccess.IResolveRootFactory resolveRootFactory, global::AllFeaturesSuccess.OptionalInterface.OptionalParameters optionalParameters);

        [global::Sundew.Injection.IndirectCreateMethodAttribute]
        global::System.Threading.Tasks.Task<global::AllFeaturesSuccess.ChildFactory.ConstructedChild> CreateAsync(global::AllFeaturesSuccess.IResolveRootFactory resolveRootFactory, global::AllFeaturesSuccess.OptionalInterface.OptionalParameters optionalParameters);

        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        [global::Sundew.Injection.IndirectCreateMethodAttribute]
        global::Sundew.Injection.Constructed<global::AllFeaturesSuccess.ChildFactory.ConstructedChild> CreateUninitialized(global::AllFeaturesSuccess.IResolveRootFactory resolveRootFactory, global::AllFeaturesSuccess.OptionalInterface.OptionalParameters optionalParameters);

        void Dispose(global::AllFeaturesSuccess.ChildFactory.ConstructedChild constructedChild);

        global::System.Threading.Tasks.ValueTask DisposeAsync(global::AllFeaturesSuccess.ChildFactory.ConstructedChild constructedChild);
    }
}
