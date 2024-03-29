﻿namespace AllFeaturesSuccess.ChildFactory;

using System;
using System.Threading.Tasks;
using AllFeaturesSuccess.NewInstance;
using AllFeaturesSuccessDependency;
using Initialization.Interfaces;

public class ConstructedChild : IAsyncInitializable, IPrint
{
    private readonly NewInstanceAndDisposable newInstanceAndDisposable;
    private readonly Dependency dependency;
    private readonly ManualDependency manualDependency;

    public ConstructedChild(NewInstanceAndDisposable newInstanceAndDisposable, Dependency dependency, ManualDependency manualDependency)
    {
        this.newInstanceAndDisposable = newInstanceAndDisposable;
        this.dependency = dependency;
        this.manualDependency = manualDependency;
    }

    public ValueTask InitializeAsync()
    {
        return default;
    }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        this.newInstanceAndDisposable.PrintMe(indent + 2);
        Console.WriteLine(new string(' ', indent + 2) + this.dependency.GetType().Name);
        Console.WriteLine(new string(' ', indent + 2) + this.manualDependency.GetType().Name);
    }
}