namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

using Sundew.Injection.Generator.TypeSystem;

internal readonly record struct Dependant(Type Type, Scope Scope);