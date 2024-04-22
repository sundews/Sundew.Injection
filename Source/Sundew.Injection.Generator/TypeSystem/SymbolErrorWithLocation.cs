namespace Sundew.Injection.Generator.TypeSystem;

using Microsoft.CodeAnalysis;

internal readonly record struct SymbolErrorWithLocation(SymbolError SymbolError, Location Location)
{
    public string GetErrorText()
    {
        return this.SymbolError.GetErrorText();
    }
}