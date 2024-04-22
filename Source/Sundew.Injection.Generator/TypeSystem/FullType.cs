namespace Sundew.Injection.Generator.TypeSystem;

internal readonly record struct FullType(Type Type, TypeMetadata Metadata, Method? DefaultConstructor);
