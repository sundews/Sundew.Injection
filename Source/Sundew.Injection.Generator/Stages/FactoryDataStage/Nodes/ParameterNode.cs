namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

internal record ParameterNode(
    DefiniteType Type,
    ParameterSource ParameterSource,
    string Name,
    TypeMetadata TypeMetadata,
    bool RequiresNewInstance,
    InjectionNode? ParentInjectionNode) : IParameterNode;