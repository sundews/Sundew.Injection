namespace AllFeaturesSuccess.UnboundType;

using System;
using AllFeaturesSuccess.NewInstance;

public class Resources : IPrint
{
    private readonly NewInstanceAndDisposable newInstanceAndDisposable;

    public Resources(NewInstanceAndDisposable newInstanceAndDisposable)
    {
        this.newInstanceAndDisposable = newInstanceAndDisposable;
    }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        this.newInstanceAndDisposable.PrintMe(indent + 2);
    }
}