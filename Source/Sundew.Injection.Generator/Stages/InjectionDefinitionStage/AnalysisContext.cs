namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using Microsoft.CodeAnalysis;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record AnalysisContext(
    SemanticModel SemanticModel,
    KnownAnalysisTypes KnownAnalysisTypes,
    TypeFactory TypeFactory,
    CompiletimeInjectionDefinitionBuilder CompiletimeInjectionDefinitionBuilder);