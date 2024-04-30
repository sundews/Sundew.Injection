namespace OverallSuccess.OptionalInterface;

using System;
using OverallSuccess.InterfaceImplementationBindings;
using OverallSuccess.NewInstance;

public record OptionalParameters(IInjectableByInterface? InjectableByInterface = null, Func<NewInstanceAndDisposable>? NewInstanceAndDisposableFactory = null);