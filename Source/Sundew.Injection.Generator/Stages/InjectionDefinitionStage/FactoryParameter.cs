namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using Sundew.Injection.Generator.TypeSystem;

internal readonly record struct FactoryParameter(Type Type, Inject Inject);
