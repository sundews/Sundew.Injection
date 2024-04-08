namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

using Microsoft.CodeAnalysis;
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
        var newDependantScopeContext = suggestedScope with { Scope = dependantScope };
        return (suggestedScope.Scope, dependantScope) switch
        {
            (Scope.Auto, _) => (newDependantScopeContext, default),
            (Scope.NewInstance, Scope.SingleInstancePerFuncResult) => (suggestedScope with { Scope = Scope._SingleInstancePerRequest(Location.None) }, GetErrorIfExplicit()),
            (Scope.NewInstance, Scope.NewInstance) => (suggestedScope, default),
            (Scope.NewInstance, _) => (newDependantScopeContext, GetErrorIfExplicit()),
            (Scope.SingleInstancePerRequest, Scope.SingleInstancePerFactory) => (newDependantScopeContext, GetErrorIfExplicit()),
            (Scope.SingleInstancePerRequest, _) => (suggestedScope, default),
            (Scope.SingleInstancePerFuncResult, Scope.SingleInstancePerFactory) => (suggestedScope, new ScopeError(targetType, suggestedScope.Scope, dependant)),
            (Scope.SingleInstancePerFuncResult, _) => (suggestedScope, default),
            (Scope.SingleInstancePerFactory, _) => (suggestedScope, default),
            (_, _) => (suggestedScope, default),
        };
    }
}