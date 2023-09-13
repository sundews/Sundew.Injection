namespace Sundew.Injection.Generator.Extensions;

using System;

internal static class ObjectExtensions
{
    public static TValue Do<TInput, TValue>(this TInput value, Action<TInput> action, TValue result)
    {
        action(value);
        return result;
    }
}