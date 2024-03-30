namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Nodes;

using Sundew.Injection.Generator.TypeSystem;

internal sealed record ThisFactoryInjectionNode(
    NamedType FactoryType,
    string? DependeeName)
    : InjectionNode(DependeeName)
{
    public override string Name => "this";
}