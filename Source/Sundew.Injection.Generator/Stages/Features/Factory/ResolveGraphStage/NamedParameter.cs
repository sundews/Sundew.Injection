namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;

using Sundew.Injection.Generator.TypeSystem;

internal readonly record struct NamedParameter(string Name, TypeMetadata Metadata, Method? DefaultConstructor);