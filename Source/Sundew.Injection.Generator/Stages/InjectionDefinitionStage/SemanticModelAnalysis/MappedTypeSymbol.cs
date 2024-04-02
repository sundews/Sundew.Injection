namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage.SemanticModelAnalysis;

using Microsoft.CodeAnalysis;

public readonly record struct MappedTypeSymbol(ITypeSymbol TypeSymbol, SyntaxNode? OriginatingSyntaxNode);
