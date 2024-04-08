namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;

using Sundew.Injection.Generator.TypeSystem;

internal sealed record ThisFactoryInjectionNode(
    NamedType FactoryType,
    string? DependantName)
    : InjectionNode(DependantName)
{
    public override string Name => "this";
}