namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

using Scope = Sundew.Injection.Generator.TypeSystem.Scope;

internal static class ScopePicker
{
    public static Scope Pick(Scope suggestedScope, Scope dependeeScope)
    {
        return suggestedScope switch
        {
            Scope.Auto => dependeeScope,
            Scope.NewInstance => dependeeScope,
            Scope.SingleInstancePerRequest => dependeeScope == Scope._NewInstance
                ? suggestedScope
                : dependeeScope,
            Scope.SingleInstancePerFuncResult => dependeeScope == Scope._NewInstance ||
                                                 dependeeScope == Scope._SingleInstancePerRequest
                ? suggestedScope
                : dependeeScope,
            Scope.SingleInstancePerFactory => dependeeScope == Scope._NewInstance ||
                                              dependeeScope == Scope._SingleInstancePerRequest ||
                                              dependeeScope is Scope.SingleInstancePerFuncResult
                ? suggestedScope
                : dependeeScope,
        };
    }
}