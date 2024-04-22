namespace Sundew.Injection.Generator.TypeSystem;

using Microsoft.CodeAnalysis;

internal interface ILocatableError
{
    Location Location { get; }
}