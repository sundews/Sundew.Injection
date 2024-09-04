namespace Sundew.Injection.Generator.TypeSystem;

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

internal readonly record struct ErrorWithLocation(Error Error, Location Location);

internal readonly record struct Error(ErrorType ErrorType, ISymbol Symbol, IReadOnlyList<Error> InnerErrors);