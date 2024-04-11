namespace Success.OptionalInterface;

using System;
using Success.InterfaceImplementationBindings;
using Success.NewInstance;

public record OptionalParameters(IInjectableByInterface? InjectableByInterface = null, Func<NewInstanceAndDisposable>? NewInstanceAndDisposableFactory = null);