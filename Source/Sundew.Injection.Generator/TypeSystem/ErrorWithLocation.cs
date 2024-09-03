namespace Sundew.Injection.Generator.TypeSystem;

using Microsoft.CodeAnalysis;

internal readonly record struct ErrorWithLocation(Error Error, Location Location);

internal readonly record struct Error(ErrorType ErrorType, SymbolError? SymbolError);