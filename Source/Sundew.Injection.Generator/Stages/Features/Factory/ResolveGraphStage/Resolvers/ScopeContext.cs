namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record ScopeContext
{
    public required Scope Scope { get; set; }

    public required ScopeOrigin Origin { get; set; }
}