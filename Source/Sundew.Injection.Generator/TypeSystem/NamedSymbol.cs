namespace Sundew.Injection.Generator.TypeSystem;

internal sealed record NamedSymbol(string Name) : ISymbol
{
    public string FullName => this.Name;
}