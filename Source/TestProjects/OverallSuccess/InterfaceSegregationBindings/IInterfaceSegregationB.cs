namespace OverallSuccess.InterfaceSegregationBindings;

using System.Diagnostics.CodeAnalysis;

public interface IInterfaceSegregationB : IPrint
{
    bool TryGet(string key, [NotNullWhen(true)] out string? value);
}