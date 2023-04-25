//HintName: IResolveRootFactory.cs
namespace AllFeaturesSuccess
{
    public interface IResolveRootFactory : global::System.IDisposable
    {
        global::AllFeaturesSuccess.IResolveRoot Create(
            int[] defaultItems, 
            global::System.Func<global::AllFeaturesSuccess.RequiredInterface.IRequiredService> requiredService, 
            global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null, 
            global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null);

        void Dispose(global::AllFeaturesSuccess.IResolveRoot resolveRoot);
    }
}
