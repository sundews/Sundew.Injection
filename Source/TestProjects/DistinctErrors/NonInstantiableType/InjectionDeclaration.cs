﻿namespace DistinctErrors.NonInstantiableType;

using Sundew.Injection;

public class InjectionDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.BindGeneric<AbstractGenericType<object>>();
    }
}

public abstract class AbstractGenericType<T>
{
}