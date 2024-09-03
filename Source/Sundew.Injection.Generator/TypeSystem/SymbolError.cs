namespace Sundew.Injection.Generator.TypeSystem;

using System.Collections.Generic;
using System.Text;
using Sundew.Base.Text;

internal readonly record struct SymbolError(ISymbol Symbol, IReadOnlyList<Error> Nodes)
{
    public string GetErrorText()
    {
        return new StringBuilder(this.Symbol.FullName).AppendItems(this.Nodes, (builder, error) => GetErrorPath(builder, error, 1)).ToString();
    }

    private static StringBuilder GetErrorPath(StringBuilder stringBuilder, Error error, int indentation)
    {
        stringBuilder.AppendLine().Append('-', indentation).Append('>').Append(' ');
        if (error.SymbolError is { } symbolError)
        {
            const string separator = " >>> ";
            return stringBuilder.Append(symbolError.Symbol.FullName).Append(separator).Append(error.ErrorType).AppendItems(symbolError.Nodes, (builder, error) => GetErrorPath(builder, error, indentation + 1));
        }

        return stringBuilder.Append(error.ErrorType);
    }
}