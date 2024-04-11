namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using Scope = Sundew.Injection.Generator.TypeSystem.Scope;

internal static class ScopePicker
{
    public static (ScopeResolverBuilder.ScopeContext Context, ScopeError? Error) Pick(Type targetType, ScopeResolverBuilder.ScopeContext suggestedScope, Dependant dependant)
    {
        ScopeError? GetErrorIfExplicit()
        {
            if (suggestedScope.Selection == ScopeSelection.Explicit)
            {
                return new ScopeError(targetType, suggestedScope.Scope, dependant);
            }

            return default;
        }

        var dependantScope = dependant.Scope;
        var newScopeFromDependantScope = suggestedScope with { Scope = dependantScope.ToDependencyScope() };
        return (suggestedScope.Scope, dependantScope) switch
        {
            (Scope.Auto, _) => (newScopeFromDependantScope, default),
            (Scope.NewInstance, Scope.SingleInstancePerFuncResult) => (newScopeFromDependantScope, GetErrorIfExplicit()),
            (Scope.NewInstance, Scope.NewInstance) => (suggestedScope, default),
            (Scope.NewInstance, _) => (newScopeFromDependantScope, GetErrorIfExplicit()),
            (Scope.SingleInstancePerRequest, Scope.SingleInstancePerFactory) => (newScopeFromDependantScope, GetErrorIfExplicit()),
            (Scope.SingleInstancePerRequest, _) => (suggestedScope, default),
            (Scope.SingleInstancePerFuncResult, Scope.SingleInstancePerFactory) => (suggestedScope, new ScopeError(targetType, suggestedScope.Scope, dependant)),
            (Scope.SingleInstancePerFuncResult, _) => (suggestedScope, default),
            (Scope.SingleInstancePerFactory, _) => (suggestedScope, default),
            (_, _) => (suggestedScope, default),
        };
    }
}