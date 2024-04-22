namespace Sundew.Injection.Generator.TypeSystem;

using System.Collections.Generic;
using System.Text;
using Sundew.Base.Text;

internal readonly record struct SymbolError(ISymbol Symbol, IReadOnlyList<SymbolError> Errors)
{
    public string GetErrorText()
    {
        return new StringBuilder(this.Symbol.FullName).AppendItems(this.Errors, (builder, error) => GetError(builder, error, 1)).ToString();
    }

    private static StringBuilder GetError(StringBuilder stringBuilder, SymbolError symbolError, int indentation)
    {
        return stringBuilder.AppendLine().Append('-', indentation).Append('>').Append(' ').Append(symbolError.Symbol.FullName)
            .AppendItems(symbolError.Errors, (builder, error) => GetError(builder, error, indentation + 1));
    }
}