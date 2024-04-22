namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using Microsoft.CodeAnalysis;

public readonly record struct TypeSymbolWithLocation(ITypeSymbol TypeSymbol, Location Location);
