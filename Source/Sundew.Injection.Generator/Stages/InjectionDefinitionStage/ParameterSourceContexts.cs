namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using Sundew.Base.Collections.Immutable;

internal readonly record struct ParameterSourceContexts(ValueArray<ParameterSource> ParameterSources, ScopeContext ScopeContext);