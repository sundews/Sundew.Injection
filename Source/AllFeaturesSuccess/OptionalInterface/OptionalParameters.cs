namespace AllFeaturesSuccess.OptionalInterface;

using System;
using AllFeaturesSuccess.InterfaceImplementationBindings;
using AllFeaturesSuccess.NewInstance;

public record OptionalParameters(IInjectableByInterface? InjectableByInterface = null, Func<NewInstanceAndDisposable>? NewInstanceAndDisposableFactory = null);