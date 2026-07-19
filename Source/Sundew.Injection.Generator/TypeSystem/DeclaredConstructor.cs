namespace Sundew.Injection.Generator.TypeSystem;

using Sundew.Base.Collections.Immutable;

internal readonly record struct DeclaredConstructor(ValueArray<FullParameter> Parameters, bool IsPublic, bool IsPartialDefinition);