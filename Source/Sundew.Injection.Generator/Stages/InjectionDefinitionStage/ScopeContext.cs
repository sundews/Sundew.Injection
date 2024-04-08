namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using Sundew.Injection.Generator.TypeSystem;

internal readonly record struct ScopeContext(Scope Scope, ScopeSelection Selection);